using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    [Header("Enemigo Cuerpo a Cuerpo --------------------")]
    public GameObject Charco;
    private Coroutine soltarCharcoCoroutine;
    private GameObject charcoInstanciado;
    public float tiempoCharcoActivo = 5f; // Tiempo que el charco permanece activo

    public bool haAlcanzadoAlJugador = false;
    public Collider triggerAtaque;
    public bool estaAtacando = false;
    public SpriteRenderer spriteRangoAtaque;

    [Header("Configuración de ataque cuerpo a cuerpo")]
    [Tooltip("Tiempo de retardo antes de ejecutar el ataque (segundos)")]
    public float delayAntesDeAtaque = 0.4f;

    [Header("Cooldown de ataque tras contacto")]
    [Tooltip("Tiempo de espera antes de poder volver a atacar tras un contacto (segundos)")]
    public float cooldownContactoConJugador = 1.5f;

    public float tiempoCargaAtaque = 1.5f; // Tiempo de carga del ataque cargado
    public float multiplicadorDañoCargado = 2f; // Daño multiplicado para el ataque cargado

    private float tiempoUltimoAtaque = -999f;

    private bool puedeAtacar = true;
    public bool enContactoConJugador = false;
    private Coroutine ataqueCoroutine;
    private int contactoPendiente = 0;

    private bool estaCargando = false; // NUEVO: para saber si está haciendo ataque cargado

    protected override void Start()
    {
        base.Start();
        // Instanciar el charco una sola vez y desactivarlo
        if (Charco != null)
        {
            charcoInstanciado = Instantiate(Charco, transform.position, Quaternion.identity);
            charcoInstanciado.SetActive(false);
        }
        soltarCharcoCoroutine = StartCoroutine(SoltarCharcoCadaIntervalo(tiempoParaSoltarObjeto));
        if (triggerAtaque != null)
        {
            triggerAtaque.enabled = false;
        }
    }

    private IEnumerator SoltarCharcoCadaIntervalo(float intervalo)
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);
            if (persiguiendoJugador && puedeSoltarObjeto)
            {
                SoltarCharco();
            }
        }
    }

    private void SoltarCharco()
    {
        if (charcoInstanciado == null)
            return;

        if (!charcoInstanciado.activeSelf)
        {
            Vector3 dropPosition = this.dropPosition != null ? this.dropPosition.position : transform.position;
            RaycastHit hit;
            if (Physics.Raycast(dropPosition, Vector3.down, out hit))
            {
                dropPosition.y = hit.point.y + 0.1f;
            }
            charcoInstanciado.transform.position = dropPosition;
            charcoInstanciado.SetActive(true);

            // Si el charco tiene un método para iniciar su ciclo, llámalo
            Charco charco = charcoInstanciado.GetComponent<Charco>();
            if (charco != null)
            {
                charco.IniciarDisminucion();
            }

            // Desactivar el charco después de un tiempo
            StartCoroutine(DesactivarCharcoTrasTiempo());
        }
    }

    private IEnumerator DesactivarCharcoTrasTiempo()
    {
        yield return new WaitForSeconds(tiempoCharcoActivo);
        if (charcoInstanciado != null)
            charcoInstanciado.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (animator != null)
            animator.SetBool("Atacando", estaAtacando);
    }

    public override void PerseguirJugador()
    {
        if (enContactoConEscudo || enContactoConJugador || estaAtacando)
        {
            velocidadMovimientoActual = 0f;
            if (animator != null)
                animator.SetBool("Caminando", false);
        }
        else
        {
            base.PerseguirJugador();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !estaCargando && !isOnWayToAderezo)
        {
            enContactoConJugador = true;

            if (soltarCharcoCoroutine != null)
            {
                StopCoroutine(soltarCharcoCoroutine);
                soltarCharcoCoroutine = null;
            }

            // Detener movimiento y comenzar ataque cargado
            if (ataqueCoroutine != null)
            {
                StopCoroutine(ataqueCoroutine);
                ataqueCoroutine = null;
            }
            ataqueCoroutine = StartCoroutine(AtaqueCargadoConBloqueo());
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isOnWayToAderezo)
        {
            OnCollisionEnter(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enContactoConJugador = false;
            estaAtacando = false;
            estaCargando = false;
            puedeAtacar = true;

            if (ataqueCoroutine != null)
            {
                StopCoroutine(ataqueCoroutine);
                ataqueCoroutine = null;
            }

            if (soltarCharcoCoroutine == null)
            {
                soltarCharcoCoroutine = StartCoroutine(SoltarCharcoCadaIntervalo(tiempoParaSoltarObjeto));
            }

            // Recuperar velocidad
            velocidadMovimientoActual = velocidadMovimientoInicial;
        }
    }

    // Corrutina para ataque cargado al entrar en contacto
    private IEnumerator AtaqueCargadoConBloqueo()
    {
        estaCargando = true;
        estaAtacando = true;
        puedeAtacar = false;
        velocidadMovimientoActual = 0f;

        if (animator != null)
            animator.SetTrigger("Atacando");

        yield return new WaitForSeconds(tiempoCargaAtaque);

        if (spriteRangoAtaque != null)
            spriteRangoAtaque.enabled = true;
        triggerAtaque.enabled = true;

        yield return new WaitForSeconds(0.4f);

        triggerAtaque.enabled = false;
        if (spriteRangoAtaque != null)
            spriteRangoAtaque.enabled = false;

        estaCargando = false;
        estaAtacando = false;
        puedeAtacar = true;
        ataqueCoroutine = null;
    }

    // Corrutina para ataque normal mientras está en contacto
    private IEnumerator AtaqueNormalEnContacto()
    {
        while (enContactoConJugador && !estaCargando)
        {
            estaAtacando = true;
            puedeAtacar = false;

            if (animator != null)
                animator.SetTrigger("Atacando");

            if (spriteRangoAtaque != null)
                spriteRangoAtaque.enabled = true;
            triggerAtaque.enabled = true;

            yield return new WaitForSeconds(0.4f);

            triggerAtaque.enabled = false;
            if (spriteRangoAtaque != null)
                spriteRangoAtaque.enabled = false;

            estaAtacando = false;
            puedeAtacar = true;

            yield return new WaitForSeconds(cooldownContactoConJugador);
        }
        ataqueCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerAtaque.enabled && other.CompareTag("Player"))
        {
            Jugador jugador = other.GetComponent<Jugador>();
            if (jugador != null)
            {
                float dañoFinal = damage;
                // Si el ataque es cargado, multiplica el daño
                if (estaAtacando && animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("CargarAtaque"))
                {
                    dañoFinal *= multiplicadorDañoCargado;
                }

                if (jugador.escudoActivo)
                {
                    jugador.ReducirResistenciaEscudo(dañoFinal);
                }
                else
                {
                    jugador.ReducirVida(dañoFinal);
                }
            }
        }
    }
}
