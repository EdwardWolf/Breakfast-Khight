using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoDistancia : Enemigo
{
    public AttackHandler attackHandler;
    public bool atacando = false;
    public Camera camara;

    [Header("Raycast de ataque")]
    public float rangoRaycast = 10f; // Distancia m�xima del raycast
    [Header("Distancia m�nima para atacar (se aleja si est� m�s cerca)")]
    [Range(0.5f, 20f)]
    public float distanciaMinimaAtaque = 6f; // Ajusta este valor en el Inspector

    public LayerMask layerJugador;   // Asigna el layer del jugador en el inspector
    public GameObject obj;
    public float offsetY = 1.0f; // Ajusta este valor seg�n lo que necesites

    private NavMeshAgent navMeshAgent;

    public GameObject objetoAActivar; // Referencia al objeto que quieres activar antes de disparar

    private Color colorOriginal;
    private bool estaActivadoPorAderezo = false;

    protected override void Start()
    {
        base.Start();
        attackHandler = GetComponent<AttackHandler>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        
        // Guardar referencia del color original si existe objetoAActivar
        if (objetoAActivar != null)
        {
            // Asegurar que el objeto est� activo al menos una vez para configurarlo
            objetoAActivar.SetActive(true);
            
            Renderer renderer = objetoAActivar.GetComponent<Renderer>();
            if (renderer != null)
            {
                colorOriginal = renderer.material.color;
            }
            
            // Despu�s, desactivarlo hasta que sea necesario
            objetoAActivar.SetActive(false);
        }
    }

    protected override void Update()
    {
        base.Update();

        // Controlar la activaci�n y color del objeto seg�n el estado
        ActualizarEstadoObjetoAActivar();

        // Si est� alej�ndose, no ejecutar l�gica de ataque ni persecuci�n
        if (alejandose)
            return;

        // Si hay un aderezo detectado y no est� interactuando con �l, perseguirlo
        if (isOnWayToAderezo && !isInteractingWithAderezo)
        {
            velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
            PerseguirAderezo();
            return;
        }

        if (jugador != null)
        {
            Collider jugadorCollider = jugador.GetComponent<Collider>();
            Vector3 centroJugador = jugadorCollider != null ? jugadorCollider.bounds.center : jugador.transform.position;
            Vector3 direccionAlJugador = (centroJugador - obj.transform.position).normalized;
            float distanciaAlJugador = Vector3.Distance(transform.position, jugador.transform.position);

            // Alejarse si est� demasiado cerca
            if (distanciaAlJugador < distanciaMinimaAtaque)
            {
                Vector3 direccionAlejarse = (transform.position - jugador.transform.position).normalized;
                Vector3 destinoAlejarse = jugador.transform.position + direccionAlejarse * distanciaMinimaAtaque;
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = statsEnemigo.velocidadMovimiento;
                navMeshAgent.SetDestination(destinoAlejarse);
                persiguiendoJugador = false;
                velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
                return;
            }
            // Acercarse si est� demasiado lejos
            else if (distanciaAlJugador > rangoRaycast)
            {
                navMeshAgent.isStopped = false;
                persiguiendoJugador = true;
                velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
                PerseguirJugador();
                return;
            }
            // Atacar si est� en rango
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(obj.transform.position, direccionAlJugador, out hit, rangoRaycast, layerJugador))
                {
                    if (!isInteractingWithAderezo)
                    {
                        if (velocidadMovimientoActual != 0f)
                            velocidadMovimientoActual = 0f;

                        navMeshAgent.isStopped = true;
                        persiguiendoJugador = false;
                        LanzarProyectilConEfecto();
                    }
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    persiguiendoJugador = true;
                    velocidadMovimientoActual = statsEnemigo.velocidadMovimiento;
                    PerseguirJugador();
                }
            }
        }
    }

    public override IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null)
        {
            // No atacar si est� alej�ndose
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
                // Si est� alej�ndose, esperar un frame antes de volver a comprobar
                yield return null;
            }
        }
        isDamaging = false;
        atacando = false;
    }

    public void LanzarProyectilConEfecto()
    {
        // No atacar si est� alej�ndose
        if (alejandose || isOnWayToAderezo || isInteractingWithAderezo)
            return;
        
        // Activar el efecto visual antes de disparar
        if (objetoAActivar != null)
            StartCoroutine(ActivarObjetoYLanzarProyectil());
        else
            attackHandler.ActivarAtaque(); // Si no hay objeto que activar, disparar directamente
    }

    private IEnumerator ActivarObjetoYLanzarProyectil()
    {
        // Activar el objeto
        objetoAActivar.SetActive(true);
        
        // Esperar un peque�o delay (puedes ajustar este valor)
        yield return new WaitForSeconds(0.2f);
        
        // Ejecutar el ataque
        if (attackHandler != null)
            attackHandler.ActivarAtaque();
        
        // Esperar otro peque�o delay antes de desactivar el objeto
        yield return new WaitForSeconds(0.5f);
        
        // Desactivar el objeto
        objetoAActivar.SetActive(false);
    }

    private void ActualizarEstadoObjetoAActivar()
    {
        if (objetoAActivar == null)
        {
            Debug.LogWarning("objetoAActivar no est� asignado");
            return;
        }
            
        if (isOnWayToAderezo)
        {
            // Asegurar que el objeto est� activo
            objetoAActivar.SetActive(true);
            
            // Cambiar color a verde
            Renderer renderer = objetoAActivar.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.green;
                Debug.Log("Cambiando color a verde, enemigo persiguiendo aderezo");
            }
            
            estaActivadoPorAderezo = true;
        }
        else if (estaActivadoPorAderezo)
        {
            // Restaurar color original
            Renderer renderer = objetoAActivar.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = colorOriginal;
            }
            
            // Solo desactivar si no est� atacando
            if (!atacando)
            {
                objetoAActivar.SetActive(false);
            }
            
            estaActivadoPorAderezo = false;
        }
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













