using System.Collections;
using UnityEngine;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler;
    public bool atacando = false;
    public GameObject objetoAActivar;
    public Camera camara;

    [Header("Raycast de ataque")]
    public float rangoRaycast = 10f; // Distancia máxima del raycast
    public LayerMask layerJugador;   // Asigna el layer del jugador en el inspector
    public GameObject obj;
    public float offsetY = 1.0f; // Ajusta este valor según lo que necesites

    protected override void Start()
    {
        base.Start();
        attackHandler = GetComponent<AttackHandler>();
    }

    protected override void Update()
    {
        base.Update();

        // Si hay un aderezo detectado y no está interactuando con él, perseguirlo
        if (isOnWayToAderezo && !isInteractingWithAderezo)
        {
            velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
            PerseguirAderezo();
            return;
        }

        if (jugador != null)
        {
            Vector3 destino = jugador.transform.position + Vector3.up * offsetY;
            Vector3 direccionAlJugador = (destino - obj.transform.position).normalized;
            float distanciaAlJugador = Vector3.Distance(transform.position, jugador.transform.position);

            // Raycast para detectar al jugador
            RaycastHit hit;
            if (Physics.Raycast(obj.transform.position, direccionAlJugador, out hit, rangoRaycast, layerJugador))
            {
                // Si el raycast impacta al jugador y no está interactuando con aderezo, detenerse y atacar
                if (!isInteractingWithAderezo)
                {
                    if (velocidadMovimientoActual != 0f)
                        velocidadMovimientoActual = 0f;

                    persiguiendoJugador = false;
                    LanzarProyectilConEfecto();
                }
            }
            else if (distanciaAlJugador <= detectionRadius)
            {
                persiguiendoJugador = true;
                velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
                PerseguirJugador();
            }
            else
            {
                persiguiendoJugador = false;
                velocidadMovimientoActual = 0f;
            }
        }
       
        

        // Hacer que el objeto siempre mire a la cámara
        if (objetoAActivar != null && objetoAActivar.activeSelf && camara != null)
        {
            objetoAActivar.transform.LookAt(
                objetoAActivar.transform.position + camara.transform.rotation * Vector3.forward,
                camara.transform.rotation * Vector3.up
            );
        }
    }

    public override IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            // No atacar si está alejándose
            if (!alejandose)
            {
                yield return new WaitForSeconds(atackCooldown);

                if (damageParticleSystem != null)
                {
                    damageParticleSystem.Stop();
                }

                if (!IsPlayerInAttackRange())
                {
                    velocidadMovimientoInicial = statsEnemigo.velocidadMovimiento;
                    PerseguirJugador();
                    break;
                }
            }
            else
            {
                // Si está alejándose, esperar un frame antes de volver a comprobar
                yield return null;
            }
        }
        isDamaging = false;
        atacando = false;
    }

    public void LanzarProyectilConEfecto()
    {
        // No atacar si está alejándose
        if (alejandose || isOnWayToAderezo || isInteractingWithAderezo)
            return;

        StartCoroutine(ActivarObjetoYLanzarProyectil());
    }

    private IEnumerator ActivarObjetoYLanzarProyectil()
    {
        if (objetoAActivar != null)
            objetoAActivar.SetActive(true);

        float delay = Random.Range(0.05f, 0.3f);
        yield return new WaitForSeconds(delay);

        if (attackHandler != null)
            attackHandler.ActivarAtaque();

        yield return new WaitForSeconds(2f);

        if (objetoAActivar != null)
            objetoAActivar.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        if (jugador == null)
            return;

        // Dibuja el raycast de ataque en color rojo
        Gizmos.color = Color.red;
        Vector3 origen = transform.position;
        Vector3 direccion = (jugador.transform.position - transform.position).normalized;
        Gizmos.DrawLine(origen, origen + direccion * rangoRaycast);
    }
}




