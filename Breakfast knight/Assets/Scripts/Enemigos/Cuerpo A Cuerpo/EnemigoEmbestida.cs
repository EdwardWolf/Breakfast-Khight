using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoEmbestida : Enemigo
{
    public float tiempoPreparacion = 2f; // Tiempo de preparación antes de la embestida
    public float velocidadEmbestida = 10f; // Velocidad de la embestida
    private Vector3 ultimaPosicionJugador; // Última posición conocida del jugador
    private bool embistiendo = false; // Indica si el enemigo está embistiendo

    private NavMeshAgent navMeshAgent; // Referencia al componente NavMeshAgent
    public LineRenderer lineRenderer; // Referencia al componente LineRenderer

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        // Configurar el LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 1f;
            lineRenderer.endWidth = 1f;
        }
    }

    protected override void Update()
    {
        if (!embistiendo)
        {
            base.Update();
            if (playerTransform != null)
            {
                LookAtPlayer();
            }
        }
    }

    protected override void DetectPlayer()
    {
        base.DetectPlayer();
        if (playerTransform != null && !embistiendo)
        {
            ultimaPosicionJugador = playerTransform.position;
            StartCoroutine(PrepararEmbestida());
        }
    }

    private void LookAtPlayer()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    private IEnumerator PrepararEmbestida()
    {
        embistiendo = true;
        // Iniciar animación de preparación
        if (animator != null)
        {
            animator.SetTrigger("PrepararEmbestida");
        }

        // Esperar el tiempo de preparación
        yield return new WaitForSeconds(tiempoPreparacion);

        // Realizar la embestida
        StartCoroutine(Embestir());
    }

    private IEnumerator Embestir()
    {
        // Iniciar animación de embestida
        if (animator != null)
        {
            animator.SetTrigger("Embestir");
        }

        navMeshAgent.speed = velocidadEmbestida;
        navMeshAgent.SetDestination(ultimaPosicionJugador);

        // Configurar el LineRenderer para proyectar la línea
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, ultimaPosicionJugador);
            lineRenderer.enabled = true;
        }

        while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > 0.1f)
        {
            // Asegurarse de que el enemigo mire hacia la posición del jugador durante la embestida
            LookAtPlayer();
            yield return null;
        }

        // Asegurarse de que el enemigo se detenga exactamente en la última posición conocida del jugador
        navMeshAgent.ResetPath();

        // Desactivar el LineRenderer
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        // Llegó al punto final
        embistiendo = false;

        // Verificar si el jugador sigue en rango
        if (IsPlayerInAttackRange())
        {
            // Regresar al estado natural y prepararse para embestir de nuevo
            DetectPlayer();
        }
    }

private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemigo ha colisionado con el jugador");
            jugador = collision.gameObject.GetComponent<Jugador>();
            if (jugador != null)
            {
                // Detener la embestida
                StopAllCoroutines();
                embistiendo = false;
                navMeshAgent.isStopped = true;

                // Desactivar el LineRenderer
                if (lineRenderer != null)
                {
                    lineRenderer.enabled = false;
                }

                // Iniciar el daño al jugador
                if (!isDamaging)
                {
                    StartCoroutine(DJugador());
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Enemigo ha dejado de colisionar con el jugador");
            jugador = null;
            isDamaging = false;
            embistiendo = false;

            // Restaurar el movimiento del enemigo
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = statsEnemigo.velocidadMovimiento;
        }
    }
}
