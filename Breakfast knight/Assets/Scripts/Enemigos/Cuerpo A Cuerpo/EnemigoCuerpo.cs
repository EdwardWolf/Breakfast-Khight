using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    [Header("Enemigo Cuerpo a Cuerpo --------------------")]
    public GameObject Charco;
    private Coroutine soltarObjetoCoroutine; // Variable para almacenar la corrutina
    public bool haAlcanzadoAlJugador = false; // Variable para controlar si ha alcanzado al jugador
    public Collider triggerAtaque; // Asigna este collider desde el inspector
    public bool estaAtacando = false;

    [Header("Configuración de ataque cuerpo a cuerpo")]
    [Tooltip("Tiempo de retardo antes de ejecutar el ataque (segundos)")]
    public float delayAntesDeAtaque = 0.4f;

    protected override void Start()
    {
        base.Start();
        soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
    }

    private IEnumerator SoltarObjetoCadaIntervalo(float intervalo)
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);
            if (persiguiendoJugador && puedeSoltarObjeto)
            {
                Vector3 dropPosition = this.dropPosition != null ? this.dropPosition.position : transform.position;
                RaycastHit hit;
                if (Physics.Raycast(dropPosition, Vector3.down, out hit))
                {
                    dropPosition.y = hit.point.y + 0.1f;
                }

                GameObject objetoInstanciado = Instantiate(Charco, dropPosition, Quaternion.identity);

                // Iniciar la disminución del albedo si el objeto instanciado tiene el componente Charco
                Charco charco = objetoInstanciado.GetComponent<Charco>();
                if (charco != null)
                {
                    charco.IniciarDisminucion();
                }
            }
        }
    }

    protected override void Update()
    {
        base.Update();
        // No es necesario detener y reiniciar la corrutina aquí
    }

    public override void PerseguirJugador()
    {
        if (enContactoConEscudo)
        {
            velocidadMovimientoActual = 0f;
        }
        else if (!estaAtacando)
        {
            base.PerseguirJugador();
        }
        else
        {
            // No moverse ni animar caminar si está atacando
            if (animator != null)
                animator.SetBool("Caminando", false);
        }
    }

    public void AlcanzarJugador()
    {
        haAlcanzadoAlJugador = true;
        velocidadMovimientoActual = 0f;
        StartCoroutine(EjecutarAtaque());
    }

    protected override void OnCollisionEnter(Collision collision)
    {

        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Escudo"))
        {
        //    enContactoConEscudo = true;
            if (soltarObjetoCoroutine != null)
            {
               StopCoroutine(soltarObjetoCoroutine);
           }
        //    if (jugador != null)
        //    {
        //        reducirResistenciaEscudoCoroutine = StartCoroutine(EsperarYReducirResistenciaEscudo());
        //    }
        }
    }

    protected override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);
        if (collision.gameObject.CompareTag("Escudo"))
        {
        //    enContactoConEscudo = false;
        //    if (reducirResistenciaEscudoCoroutine != null)
        //    {
        //        StopCoroutine(reducirResistenciaEscudoCoroutine);
        //        reducirResistenciaEscudoCoroutine = null;
        //    }
            if (soltarObjetoCoroutine == null)
           {
               soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
           }
        }
    }

    private IEnumerator EjecutarAtaque()
    {
        estaAtacando = true;
        // Espera antes de atacar (delay configurable)
        yield return new WaitForSeconds(delayAntesDeAtaque);

        // Inicia la animación de ataque
        if (animator != null)
            animator.SetBool("Atacando", true);

        // Activa el trigger de ataque durante el tiempo del golpe
        triggerAtaque.enabled = true;
        yield return new WaitForSeconds(0.3f); // Duración del golpe, ajústalo según la animación

        // Termina el ataque
        triggerAtaque.enabled = false;
        if (animator != null)
            animator.SetBool("Atacando", false);

        // Cooldown antes de poder moverse/atacar de nuevo
        yield return new WaitForSeconds(1f);

        haAlcanzadoAlJugador = false;
        estaAtacando = false;
        velocidadMovimientoActual = velocidadMovimientoInicial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerAtaque.enabled && other.CompareTag("Player"))
        {
            Jugador jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                jugador.ReducirVida(damage); // Usa el campo damage para coherencia
            }
        }
    }
}
