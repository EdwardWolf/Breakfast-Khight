using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    [Header("Enemigo Cuerpo a Cuerpo --------------------")]
    public GameObject Charco;
    private Coroutine soltarCharcoCoroutine;
    private GameObject charcoInstanciado;
    public float tiempoCharcoActivo = 5f;

    public bool haAlcanzadoAlJugador = false;
    public bool estaAtacando = false;
    public SpriteRenderer spriteRangoAtaque;

    [Header("Configuración de ataque cuerpo a cuerpo")]
    [Tooltip("Tiempo de retardo antes de ejecutar el ataque (segundos)")]
    public float delayAntesDeAtaque = 0.4f;

    public float tiempoCargaAtaque = 1.5f;
    public float multiplicadorDañoCargado = 2f;

    public bool enContactoConJugador = false;
    private Coroutine ataqueCoroutine;
    private bool estaCargando = false;

    public Collider triggerAtaque;

    // NUEVO: Control de cooldown entre ataques
    public float cooldownContactoConJugador = 1.5f;
    private float tiempoUltimoAtaque = -999f;

    public GameObject objetoAActivar; // Objeto visual que aparecerá antes del ataque

    protected override void Start()
    {
        base.Start();
        if (Charco != null)
        {
            charcoInstanciado = Instantiate(Charco, transform.position, Quaternion.identity);
            charcoInstanciado.SetActive(false);
        }
        soltarCharcoCoroutine = StartCoroutine(SoltarCharcoCadaIntervalo(tiempoParaSoltarObjeto));
        if (triggerAtaque != null)
            triggerAtaque.enabled = false;
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

            Charco charco = charcoInstanciado.GetComponent<Charco>();
            if (charco != null)
            {
                charco.IniciarDisminucion();
            }
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
        if (collision.gameObject.CompareTag("Player") && !estaCargando)
        {
            enContactoConJugador = true;

            if (ataqueCoroutine != null)
            {
                StopCoroutine(ataqueCoroutine);
                ataqueCoroutine = null;
            }
            ataqueCoroutine = StartCoroutine(AtaqueCargado());
            tiempoUltimoAtaque = Time.time;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enContactoConJugador = true;

            // Si no está atacando/cargando y ha pasado el cooldown, ejecuta otro ataque
            if (!estaCargando && !estaAtacando && Time.time - tiempoUltimoAtaque >= cooldownContactoConJugador)
            {
                if (ataqueCoroutine != null)
                {
                    StopCoroutine(ataqueCoroutine);
                    ataqueCoroutine = null;
                }
                ataqueCoroutine = StartCoroutine(AtaqueCargado());
                tiempoUltimoAtaque = Time.time;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            enContactoConJugador = false;
            if (!estaAtacando)
                velocidadMovimientoActual = velocidadMovimientoInicial;
        }
    }

    private IEnumerator AtaqueCargado()
    {
        estaCargando = true;
        velocidadMovimientoActual = 0f;

        // Nota: Ya NO activamos la animación de ataque aquí, sino cuando realmente ataca

        // Mostrar objeto visual durante la carga
        if (objetoAActivar != null)
            StartCoroutine(MostrarObjetoVisual(0.2f, tiempoCargaAtaque + delayAntesDeAtaque - 0.1f));

        if (spriteRangoAtaque != null)
            spriteRangoAtaque.enabled = true;

        // Fase de carga - el enemigo está preparándose para atacar
        yield return new WaitForSeconds(tiempoCargaAtaque);
        yield return new WaitForSeconds(delayAntesDeAtaque);

        // AQUÍ ES DONDE REALMENTE ATACA - activamos la animación justo antes del collider
        estaAtacando = true;
        if (animator != null)
            animator.SetTrigger("Atacando"); // Movido aquí para sincronizar con el daño real

        // Activar el collider que aplica el daño
        if (triggerAtaque != null)
            triggerAtaque.enabled = true;

        // Mantenemos el collider activo por un tiempo
        yield return new WaitForSeconds(0.4f);

        // Desactivar el collider cuando termina el ataque
        if (triggerAtaque != null)
            triggerAtaque.enabled = false;

        if (spriteRangoAtaque != null)
            spriteRangoAtaque.enabled = false;

        // Terminó tanto la carga como el ataque
        estaCargando = false;
        estaAtacando = false;
        ataqueCoroutine = null;

        if (!enContactoConJugador)
            velocidadMovimientoActual = velocidadMovimientoInicial;
    }

    private IEnumerator MostrarObjetoVisual(float delayAntes, float duracion)
    {
        yield return new WaitForSeconds(delayAntes);
        objetoAActivar.SetActive(true);
        yield return new WaitForSeconds(duracion);
        objetoAActivar.SetActive(false);
    }
}
