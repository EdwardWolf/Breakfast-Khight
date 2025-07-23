using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour
{
    [Header("Enemigo Basics ---------------------------")]
    public EnemigoStats statsEnemigo;
    public float detectionRadius;
    public float attackRadius;
    public LayerMask playerLayer;
    public LayerMask aderezoLayer;
    public Transform playerTransform;
    public float vidaE;
    public float damage;
    public float velocidadMovimientoInicial;
    public float velocidadMovimientoActual;
    public bool usarRadioDeAtaque = true;
    public Image barraDeVida;
    private Camera camara;
    public List<GameObject> aderezosPrefabis;
    public float dropChanceMin = 0.1f;
    public float dropChanceMax = 0.5f;
    public Transform dropPosition;
    public Jugador jugador;
    public bool isDamaging = false;
    public ParticleSystem damageParticleSystem;
    public float atackCooldown = 1.5f;
    public float tiempoParaSoltarObjeto = 5f;
    public bool persiguiendoJugador = false;
    public bool puedeSoltarObjeto = true;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip golpeClip;
    public Image barraDeVidaFondo;
    public float tiempoBarraVisible = 2f;
    public float tiempoUltimoDanio = -999f;
    private bool barraVisible = false;
    public bool yaTomoAderezo = false;

    [Range(1, 10)]
    public float probabilidadUsoAderezo = 3f;

    private Transform aderezoTransform;
    public Image interactionProgressBar;

    public bool isInteractingWithAderezo = false;
    public bool isOnWayToAderezo = false;
    private float interactionTimer = 0f;
    public float interactionTime = 2f;

    [Range(0, 1)]
    public float probabilidadAlejarse = 0.3f;
    public float distanciaAlejarse = 5f; // Usada para calcular el destino de alejamiento
    public bool puedeAlejarse = true;

    private Coroutine dañoCoroutine;
    public Coroutine reducirResistenciaEscudoCoroutine;

    public float fuerzaEmpuje = 5f;

    private EnemySpawner spawner;

    public Collider escudoCollider;

    public bool isStunned = false;
    public float stunDuration = 2f;
    private Coroutine stunCoroutine;

    private Renderer enemigoRenderer;
    private Material materialOriginal;

    public float tiempoDeActivacion = 2f;
    private bool puedeActuar = false;

    public GameObject particulasDerrotaPrefab;
    public Transform puntoParticulasDerrota;

    public bool alejandose = false;
    public float tiempoAlejarse = 1.5f;
    public float tiempoAlejarseActual = 0f;

    private Vector3 posicionJugadorAlAlejarse;
    private Vector3 ultimaDireccionAlJugador;
    private NavMeshAgent agent;
    private Vector3 destinoAlejarse;
    private Vector3 ultimaPosicionJugador;

    protected virtual void Awake()
    {
        camara = Camera.main;
        if (camara != null)
        {
            audioSource = camara.GetComponent<AudioSource>();
        }
        enemigoRenderer = GetComponentInChildren<Renderer>();
        if (enemigoRenderer != null)
        {
            materialOriginal = enemigoRenderer.material;
        }
        agent = GetComponent<NavMeshAgent>();
    }

    public void OnEnable()
    {
        vidaE = statsEnemigo.vida;
        puedeActuar = false;
        StartCoroutine(ActivarTrasDelay());
    }

    protected virtual void Start()
    {
        vidaE = statsEnemigo.vida;
        ActualizarBarraDeVida();
        velocidadMovimientoInicial = statsEnemigo.velocidadMovimiento;
        velocidadMovimientoActual = velocidadMovimientoInicial;
        damage = statsEnemigo.daño;
        animator = GetComponentInChildren<Animator>();
        spawner = GetComponentInParent<EnemySpawner>();

        if (barraDeVida != null) barraDeVida.gameObject.SetActive(false);
        if (barraDeVidaFondo != null) barraDeVidaFondo.gameObject.SetActive(false);
        barraVisible = false;
    }

    protected virtual void Update()
    {
        if (!puedeActuar)
            return;

        if (isStunned)
        {
            if (animator != null)
                animator.SetBool("Caminando", false);
            return;
        }

        if (alejandose)
        {
            Alejarse();
            return;
        }

        barraDeVida.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                         camara.transform.rotation * Vector3.up);

        if (barraDeVidaFondo != null)
        {
            barraDeVidaFondo.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                                              camara.transform.rotation * Vector3.up);
        }

        DetectPlayer();
        if (aderezoTransform != null)
        {
            if (aderezoTransform.gameObject.activeSelf)
            {
                PerseguirAderezo();
            }
            else
            {
                aderezoTransform = null;
            }
        }
        else if (playerTransform != null)
        {
            LookAtPlayer();
            if (!persiguiendoJugador)
            {
                persiguiendoJugador = true;
            }
            PerseguirJugador();
        }
        else
        {
            persiguiendoJugador = false;
            if (animator != null)
                animator.SetBool("Caminando", false);
        }

        if (escudoCollider != null && !escudoCollider.enabled)
        {
            enContactoConEscudo = false;
            escudoCollider = null;
            if (reducirResistenciaEscudoCoroutine != null)
            {
                StopCoroutine(reducirResistenciaEscudoCoroutine);
                reducirResistenciaEscudoCoroutine = null;
            }
        }

        if (barraDeVida != null)
        {
            barraDeVida.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                             camara.transform.rotation * Vector3.up);
        }
        if (barraDeVidaFondo != null)
        {
            barraDeVidaFondo.transform.LookAt(transform.position + camara.transform.rotation * Vector3.forward,
                                              camara.transform.rotation * Vector3.up);
        }

        if (barraVisible && (Time.time - tiempoUltimoDanio > tiempoBarraVisible))
        {
            OcultarBarrasVida();
        }

        if (agent != null)
            agent.speed = velocidadMovimientoActual;
    }

    public void IncrementarAtaque(float cantidad)
    {
        damage += cantidad;
    }

    protected virtual void DetectPlayer()
    {
        Collider[] playerHits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (playerHits.Length > 0)
        {
            playerTransform = playerHits[0].transform;
            jugador = playerHits[0].GetComponent<Jugador>();
            if (animator != null)
            {
                animator.SetTrigger("DetectarJugador");
            }
        }
        else
        {
            playerTransform = null;
            jugador = null;
        }

        if (!alejandose && aderezoTransform == null && !yaTomoAderezo)
        {
            Collider[] aderezoHits = Physics.OverlapSphere(transform.position, detectionRadius, aderezoLayer);
            foreach (var hit in aderezoHits)
            {
                Aderezo aderezo = hit.GetComponent<Aderezo>();
                if (aderezo != null && aderezo.EstaDisponible())
                {
                    if (Random.Range(1, 11) <= probabilidadUsoAderezo)
                    {
                        aderezoTransform = aderezo.transform;
                        aderezo.enemigoQuePersigue = this;
                    }
                    break;
                }
            }
        }
    }

    public bool IsPlayerInAttackRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, playerLayer);
        return hits.Length > 0;
    }

    public virtual void Atacck()
    {
        if (animator != null)
        {
            animator.SetBool("Atacando", true);
            StartCoroutine(ResetAtaqueAnimacion(0.7f));
        }
        AplicarDanioAlJugador();
    }

    private IEnumerator ResetAtaqueAnimacion(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        if (animator != null)
        {
            animator.SetBool("Atacando", false);
        }
    }

    public void AplicarDanioAlJugador()
    {
        if (jugador != null)
        {
            jugador.ReducirVida(damage);
            if (audioSource != null && golpeClip != null)
            {
                audioSource.PlayOneShot(golpeClip);
            }
        }
    }

    public virtual IEnumerator DJugador()
    {
        isDamaging = true;
        while (jugador != null && isDamaging)
        {
            if (!alejandose)
            {
                Atacck();
                Debug.Log("Jugador ha recibido daño");

                if (damageParticleSystem != null)
                    damageParticleSystem.Play();
            }

            yield return new WaitForSeconds(atackCooldown);

            if (damageParticleSystem != null)
                damageParticleSystem.Stop();
        }
        isDamaging = false;
    }

    void LookAtPlayer()
    {
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    void LookAtAderezo()
    {
        Vector3 direction = (aderezoTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }
    public virtual void PerseguirJugador()
    {
        if (alejandose) // Protección extra: no perseguir si está alejándose
            return;

        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            float distancia = Vector3.Distance(transform.position, playerTransform.position);

            bool estaCaminando = velocidadMovimientoActual > 0.01f && distancia > 0.05f;
            if (animator != null)
                animator.SetBool("Caminando", estaCaminando);

            transform.position += direction * velocidadMovimientoActual * Time.deltaTime;
        }
        else
        {
            if (animator != null)
                animator.SetBool("Caminando", false);
        }
    }

    public void PerseguirAderezo()
    {
        if (yaTomoAderezo)
        {
            isInteractingWithAderezo = false;
            interactionTimer = 0f;
            UpdateProgressBar(0f);
            isOnWayToAderezo = false;
            aderezoTransform = null;
            if (interactionProgressBar != null)
            {
                interactionProgressBar.gameObject.SetActive(false);
            }
            if (animator != null)
                animator.SetBool("Caminando", false);
            return;
        }
        if (aderezoTransform == null || !aderezoTransform.gameObject.activeSelf)
        {
            isInteractingWithAderezo = false;
            interactionTimer = 0f;
            UpdateProgressBar(0f);
            isOnWayToAderezo = false;
            aderezoTransform = null;
            if (interactionProgressBar != null)
            {
                interactionProgressBar.gameObject.SetActive(false);
            }
            if (animator != null)
                animator.SetBool("Caminando", false);

            DetectPlayer();
            if (playerTransform != null)
            {
                persiguiendoJugador = true;
            }

            Debug.Log("Aderezo desaparecido, enemigo vuelve a perseguir al jugador");
            return;
        }

        LookAtAderezo();

        if (!isInteractingWithAderezo)
        {
            isOnWayToAderezo = true;
            Vector3 direction = (aderezoTransform.position - transform.position).normalized;
            float distancia = Vector3.Distance(transform.position, aderezoTransform.position);

            bool estaCaminando = velocidadMovimientoActual > 0.01f && distancia > 0.05f;
            if (animator != null)
                animator.SetBool("Caminando", estaCaminando);

            transform.position += direction * velocidadMovimientoActual * Time.deltaTime;

            if (distancia < 0.6f)
            {
                isInteractingWithAderezo = true;
                isOnWayToAderezo = false;
                interactionTimer = 0f;

                Aderezo aderezo = aderezoTransform.GetComponent<Aderezo>();
                if (aderezo != null)
                {
                    interactionProgressBar = aderezo.interactionProgressBar;
                    interactionProgressBar.gameObject.SetActive(true);
                }
                if (animator != null)
                    animator.SetBool("Caminando", false);
            }
        }
        else
        {
            interactionTimer += Time.deltaTime;
            UpdateProgressBar(interactionTimer / interactionTime);

            if (interactionTimer >= interactionTime)
            {
                Destroy(aderezoTransform.gameObject);
                aderezoTransform = null;
                isInteractingWithAderezo = false;
                interactionTimer = 0f;
                UpdateProgressBar(0f);
                if (interactionProgressBar != null)
                {
                    interactionProgressBar.gameObject.SetActive(false);
                }
                if (animator != null)
                    animator.SetBool("Caminando", false);
            }
        }
    }

    private void UpdateProgressBar(float progress)
    {
        if (interactionProgressBar != null)
        {
            interactionProgressBar.fillAmount = progress;
        }
    }

    private void CancelarInteraccionAderezo()
    {
        isInteractingWithAderezo = false;
        interactionTimer = 0f;
        UpdateProgressBar(0f);
        if (interactionProgressBar != null)
        {
            interactionProgressBar.gameObject.SetActive(false);
        }
        isOnWayToAderezo = false;
        aderezoTransform = null;
    }

    public void AplicarStun(float duracion)
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(duracion));
    }

    private IEnumerator StunCoroutine(float duracion)
    {
        isStunned = true;
        velocidadMovimientoActual = 0f;
        if (animator != null)
            animator.SetTrigger("Stun");

        yield return new WaitForSeconds(duracion);

        isStunned = false;
        velocidadMovimientoActual = velocidadMovimientoInicial;
    }

    public void RecibirDanio(float cantidad)
    {
        vidaE -= cantidad;
        ActualizarBarraDeVida();
        MostrarBarrasVida();
        tiempoUltimoDanio = Time.time;

        if (isInteractingWithAderezo)
        {
            CancelarInteraccionAderezo();
            AplicarStun(stunDuration);
        }

        if (vidaE <= 0)
        {
            Debug.Log($"El enemigo {gameObject.name} ha sido derrotado.");

            if (particulasDerrotaPrefab != null && puntoParticulasDerrota != null)
            {
                Instantiate(particulasDerrotaPrefab, puntoParticulasDerrota.position, puntoParticulasDerrota.rotation);
            }
            else if (particulasDerrotaPrefab != null)
            {
                Instantiate(particulasDerrotaPrefab, transform.position, Quaternion.identity);
            }

            LiberarAderezo();

            DropAderezo();
            DesactivarEnemigo();
        }
        else
        {
            if (puedeAlejarse && Random.value <= probabilidadAlejarse)
            {
                alejandose = true;
                tiempoAlejarseActual = 0f;
                velocidadMovimientoActual = velocidadMovimientoInicial; // ← RESTAURADO: velocidad normal al alejarse
                if (jugador != null)
                {
                    ultimaPosicionJugador = jugador.transform.position;
                    Vector3 direccionOpuesta = -(jugador.transform.position - transform.position).normalized;
                    if (direccionOpuesta == Vector3.zero)
                        direccionOpuesta = -transform.forward;
                    destinoAlejarse = transform.position + direccionOpuesta * distanciaAlejarse;
                }
                if (agent != null) agent.ResetPath();
            }
        }
    }

    private void Alejarse()
    {
        velocidadMovimientoActual = velocidadMovimientoInicial*2; // ← RESTAURADO: velocidad normal al alejarse
        if (agent != null)
            agent.speed = velocidadMovimientoActual;

        tiempoAlejarseActual += Time.deltaTime;

        float distanciaAlDestino = Vector3.Distance(transform.position, destinoAlejarse);
        if (distanciaAlDestino < 0.3f || tiempoAlejarseActual >= tiempoAlejarse)
        {
            alejandose = false;
            velocidadMovimientoActual = velocidadMovimientoInicial; // ← RESTAURADO: velocidad normal al terminar de alejarse
            if (agent != null) agent.ResetPath();
            return;
        }

        if (agent != null)
        {
            agent.SetDestination(destinoAlejarse);
        }

        if (animator != null)
            animator.SetBool("Caminando", true);

        Vector3 alejarseDireccion = (destinoAlejarse - transform.position).normalized;
        if (alejarseDireccion != Vector3.zero)
        {
            Quaternion rotacion = Quaternion.LookRotation(alejarseDireccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacion, Time.deltaTime * 5f);
        }
    }

    private Coroutine barraFondoCoroutine;

    public void ActualizarBarraDeVida()
    {
        if (barraDeVida != null)
        {
            barraDeVida.fillAmount = vidaE / statsEnemigo.vida;
        }
        if (barraDeVidaFondo != null)
        {
            if (barraFondoCoroutine != null)
                StopCoroutine(barraFondoCoroutine);
            barraFondoCoroutine = StartCoroutine(ActualizarBarraFondo());
        }
    }

    private IEnumerator ActualizarBarraFondo()
    {
        float objetivo = vidaE / statsEnemigo.vida;
        float velocidad = 0.5f;
        while (barraDeVidaFondo.fillAmount > objetivo)
        {
            barraDeVidaFondo.fillAmount = Mathf.MoveTowards(barraDeVidaFondo.fillAmount, objetivo, velocidad * Time.deltaTime);
            yield return null;
        }
        barraDeVidaFondo.fillAmount = objetivo;
    }

    public void DesactivarEnemigo()
    {
        gameObject.SetActive(false);

        if (spawner != null)
        {
            spawner.RegresarEnemigo(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        if (usarRadioDeAtaque)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, attackRadius);
        }
    }

    private void DropAderezo()
    {
        float dropChance = Random.Range(dropChanceMin, dropChanceMax);
        if (Random.value <= dropChance && aderezosPrefabis.Count > 0)
        {
            int randomIndex = Random.Range(0, aderezosPrefabis.Count);
            GameObject aderezoToDrop = aderezosPrefabis[randomIndex];

            if (aderezoToDrop.name.Contains("Aderezo (Mermelada)") && jugador != null && jugador.vidaActual >= jugador.stats.vida)
            {
                Debug.Log("No se dropea aderezo de salud porque el jugador tiene la vida completa.");
                return;
            }

            Vector3 spawnPos = dropPosition != null ? dropPosition.position : transform.position;
            GameObject instancia = Instantiate(aderezoToDrop, spawnPos, Quaternion.identity);

            Rigidbody rb = instancia.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direccion = (Random.onUnitSphere + Vector3.up).normalized;
                direccion.y = Mathf.Abs(direccion.y);
                float fuerza = 5f;
                rb.AddForce(direccion * fuerza, ForceMode.Impulse);
            }
        }
    }

    public float shieldDamage = 10f;
    public bool enContactoConEscudo = false;

    private IEnumerator EsperarYHacerDanio()
    {
        yield return new WaitForSeconds(2f);
        if (jugador != null)
        {
            dañoCoroutine = StartCoroutine(DJugador());
        }
    }

    public IEnumerator EsperarYReducirResistenciaEscudo()
    {
        while (enContactoConEscudo)
        {
            yield return new WaitForSeconds(2f);
            if (jugador != null && escudoCollider != null)
            {
                jugador.ReducirResistenciaEscudo(shieldDamage);
            }
        }
    }

    public SectionManager sectionManager;

    private Coroutine ralentizacionCoroutine;
    public void AplicarRalentizacion(float factor, float duracion)
    {
        if (ralentizacionCoroutine != null)
        {
            StopCoroutine(ralentizacionCoroutine);
        }
        ralentizacionCoroutine = StartCoroutine(Ralentizar(factor, duracion));
    }
    private IEnumerator Ralentizar(float factor, float duracion)
    {
        velocidadMovimientoActual *= factor;
        yield return new WaitForSeconds(duracion);
        velocidadMovimientoActual = velocidadMovimientoInicial;
    }

    private void MostrarBarrasVida()
    {
        if (!barraVisible)
        {
            if (barraDeVida != null) barraDeVida.gameObject.SetActive(true);
            if (barraDeVidaFondo != null) barraDeVidaFondo.gameObject.SetActive(true);
            barraVisible = true;
        }
    }

    private void OcultarBarrasVida()
    {
        if (barraVisible)
        {
            if (barraDeVida != null) barraDeVida.gameObject.SetActive(false);
            if (barraDeVidaFondo != null) barraDeVidaFondo.gameObject.SetActive(false);
            barraVisible = false;
        }
    }

    public void Resetear()
    {
        vidaE = statsEnemigo.vida;
        velocidadMovimientoInicial = statsEnemigo.velocidadMovimiento;
        velocidadMovimientoActual = velocidadMovimientoInicial;
        damage = statsEnemigo.daño;
        playerTransform = null;
        jugador = null;
        aderezoTransform = null;
        isDamaging = false;
        isStunned = false;
        stunCoroutine = null;
        yaTomoAderezo = false;
        puedeSoltarObjeto = true;
        persiguiendoJugador = false;
        isInteractingWithAderezo = false;
        isOnWayToAderezo = false;
        interactionTimer = 0f;
        escudoCollider = null;
        enContactoConEscudo = false;
        reducirResistenciaEscudoCoroutine = null;
        ralentizacionCoroutine = null;
        barraVisible = false;
        tiempoUltimoDanio = -999f;

        if (animator != null)
        {
            animator.SetBool("Caminando", false);
            animator.SetTrigger("Atacando");
            animator.SetTrigger("Reset");
        }

        if (barraDeVida != null) barraDeVida.gameObject.SetActive(false);
        if (barraDeVidaFondo != null) barraDeVidaFondo.gameObject.SetActive(false);

        if (enemigoRenderer != null && materialOriginal != null)
        {
            enemigoRenderer.material = materialOriginal;
        }
    }

    private IEnumerator ActivarTrasDelay()
    {
        yield return new WaitForSeconds(tiempoDeActivacion);
        puedeActuar = true;
    }
    public Vector3 GetNavMeshPosition(Vector3 origen, float maxDistance = 2f)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(origen, out hit, maxDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return origen;
    }

    void LiberarAderezo()
    {
        if (aderezoTransform != null)
        {
            Aderezo aderezo = aderezoTransform.GetComponent<Aderezo>();
            if (aderezo != null && aderezo.enemigoQuePersigue == this)
            {
                aderezo.LiberarParaEnemigos();
            }
            aderezoTransform = null;
        }
    }
}
