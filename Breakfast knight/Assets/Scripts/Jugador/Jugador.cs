using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public abstract class Jugador : MonoBehaviour
{
    [SerializeField] public Stats stats;
    [SerializeField] public float vidaActual;
    [SerializeField] protected float resistenciaEscudoActual;
    [SerializeField] public float _velocidadMovimiento;
    [SerializeField] protected float velocidadAtaque;
    [SerializeField] public float ataque; // Añadir una variable para el ataque
    private int golpesRestantes; // Variable para almacenar los golpes restantes con el ataque incrementado
    public GameObject derrota; // Referencia al objeto de derrota
    public float areaRadius = 5f; // Radio del área de efecto del ataque cargado
    public float pushForce = 5f; // Fuerza de empuje del ataque cargado
    public Image cargaBarra; // Referencia a la barra de carga
    public Image barraResistenciaEscudo; // Referencia a la barra de resistencia del escudo
    public GameObject ataqueCargadoEfecto; // Referencia al objeto del efecto del ataque cargado
    public bool isCharging = false;
    public float chargeTime;
    public float maxChargeTime = 3f; // Tiempo máximo de carga
    public bool escudoActivo = false;
    private float tiempoRegeneracion = 5f; // Tiempo de espera para comenzar la regeneración
    private float velocidadRegeneracion = 5f; // Velocidad de regeneración de la resistencia del escudo
    public float valorMinimoEscudo = 10f; // Valor mínimo de resistencia del escudo para poder usarlo
    private const float valorCorazon = 10f; // Valor de cada corazón
    public float tiempoUltimoDaño = 2f;
    public Camera camara; // Referencia a la cámara principal
    public GameObject panelPausa; // Referencia al panel de pausa
    [SerializeField] private Transform puntoInstanciacionArma; // Punto de instanciación del arma
    public Renderer EscudoR; // Referencia al renderer del escudo

    public UIManager uiManager;

    private int corazonesActuales;
    private float incrementoAtaqueTemporal; // Variable para almacenar el incremento de ataque temporal

    public delegate void VidaCambiada(int corazones);
    public static event VidaCambiada OnVidaCambiada;

    private GameInputs playerInputActions;
    public bool debufoVelocidadAplicado = false; // Variable para rastrear el debufo de velocidad

    private Coroutine regeneracionEscudoCoroutine; // Variable para almacenar la corrutina de regeneración del escudo
    private Coroutine debufoVelocidadCoroutine; // Variable para almacenar la corrutina del debufo de velocidad

    [SerializeField] private AudioSource audioSource; // Referencia al componente AudioSource
    [SerializeField] private AudioClip sonidoGolpeEscudo; // Clip de audio para el sonido del golpe en el escudo
    [SerializeField] private AudioClip sonidoAtaque; // Clip de audio para el sonido del ataque

    public bool juegoPausado = false; // Variable para rastrear el estado del juego (pausado o no)

    // Referencias a las imágenes de las armas en la UI
    public Image[] imagenesArmas;

    public GameObject[] prefabsArmas = new GameObject[3];
    public Arma[] armas = new Arma[3];
    public int armaActual = 0;

    private bool ralentizadoBala = false; //Variable para decir si el jugador esta relentizado por la bala
    public bool aturdidoBala = false; //Variable para decir si el jugador esta aturdido por la bala

    public Collider armaCollider; // Referencia al Collider del arma actual

    private bool invulnerable = false; // Variable para rastrear el estado de invulnerabilidad
    [SerializeField] private Material materialNormal; // Material normal del jugador
    [SerializeField] private Material materialDaño; // Material del jugador al recibir daño
    public List<Renderer> renderers; // Lista de renderers del jugador
    public List<Renderer> renderersNoAfectados; // Lista de renderers que no deben cambiar de material

    public float velocidadActual;

    public GameObject Equis;

    protected virtual void Start()
    {
        Equis.SetActive(false); // Desactivar el objeto "Equis" al inicio
        camara = Camera.main; // Obtener la cámara principal
        audioSource = camara.GetComponent<AudioSource>(); // Obtener el AudioSource de la cámara principal
        panelPausa.SetActive(false); // Ocultar el panel de pausa
        vidaActual = stats.vida;
        resistenciaEscudoActual = stats.resistenciaEscudo;
        _velocidadMovimiento = stats.velocidadMovimiento;
        velocidadActual = _velocidadMovimiento;
        velocidadAtaque = stats.velocidadAtaque;
        corazonesActuales = Mathf.CeilToInt(vidaActual / valorCorazon);
        OnVidaCambiada?.Invoke(corazonesActuales);
        derrota.SetActive(false);

        if (cargaBarra != null)
        {
            cargaBarra.fillAmount = 0f; // Inicializar la barra de carga
        }

        if (barraResistenciaEscudo != null)
        {
            barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo; // Inicializar la barra de resistencia del escudo
        }

        if (ataqueCargadoEfecto != null)
        {
            ataqueCargadoEfecto.SetActive(false); // Inicializar el efecto del ataque cargado como deshabilitado
        }

        playerInputActions = new GameInputs();
        playerInputActions.Player.AtaqueCargado.started += OnAttackChargedStarted;
        playerInputActions.Player.AtaqueCargado.canceled += OnAttackChargedCanceled;
        playerInputActions.Player.Attack.started += OnAttackStarted;
        playerInputActions.Player.Pausa.started += OnPausePressed;
        playerInputActions.Player.ArmaAnterior.started += OnArmaAnterior; // Asignar la acción de cambiar al arma anterior
        playerInputActions.Player.ArmaSiguiente.started += OnArmaSiguiente; // Asignar la acción de cambiar al arma siguiente

        playerInputActions.Enable(); // Asegúrate de habilitar los controles aquí

        renderersNoAfectados = new List<Renderer>(); // Inicializar la lista de renderers no afectados

        for (int i = 0; i < prefabsArmas.Length; i++)
        {
            if (prefabsArmas[i] != null)
            {
                GameObject armaInstanciada = Instantiate(prefabsArmas[i], puntoInstanciacionArma);
                armas[i] = armaInstanciada.GetComponent<Arma>();
                armaInstanciada.SetActive(i == armaActual);

                // Agregar los renderers de las armas a la lista de renderers no afectados
                Renderer[] armaRenderers = armaInstanciada.GetComponentsInChildren<Renderer>();
                renderersNoAfectados.AddRange(armaRenderers);
            }
        }
        // Asignar el valor de ataque del arma equipada
        if (armas[armaActual] != null)
        {
            ataque = armas[armaActual].daño;
        }

        ActualizarUIArmas();
        ActualizarArmaCollider(); // Actualizar el Collider del arma actual
                                  // Agregar el renderer del escudo a la lista de renderers no afectados
        Renderer escudoRenderer = EscudoR.GetComponent<Renderer>();
        if (escudoRenderer != null)
        {
            renderersNoAfectados.Add(escudoRenderer);
        }

        // Agregar los renderers de las partículas a la lista de renderers no afectados
        ParticleSystemRenderer[] particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>();
        renderersNoAfectados.AddRange(particleRenderers);

        renderers = new List<Renderer>(GetComponentsInChildren<Renderer>());
    }


    public void AplicarRalentizacion(float factor, float duracion)
    {
        if (!ralentizadoBala)
        {
            StartCoroutine(Ralentizar(factor, duracion));
        }
    }

    private IEnumerator Ralentizar(float factor, float duracion)
    {
        ralentizadoBala = true;
       velocidadActual = velocidadActual * factor;

        // Asigna la velocidad reducida al jugador

        yield return new WaitForSeconds(duracion);

        // Restaura la velocidad original del jugador
        ralentizadoBala = false;
        velocidadActual = _velocidadMovimiento;
    }

    public void AplicarAturdimiento(float duracion)
    {
        if (!aturdidoBala)
        {
            StartCoroutine(Aturdir(duracion));
        }
    }

    private IEnumerator Aturdir(float duracion)
    {
        aturdidoBala = true;
        velocidadActual = 0f;

        // Asigna la velocidad reducida al jugador
        yield return new WaitForSeconds(duracion);
        // Restaura la velocidad original del jugador
        aturdidoBala = false;
        velocidadActual = _velocidadMovimiento;
    }

    public void AplicarDebufoVelocidad(float reduccionVelocidad, float duracion)
    {
        if (!debufoVelocidadAplicado)
        {
            velocidadActual *= (1 - reduccionVelocidad);
            debufoVelocidadAplicado = true;
            if (debufoVelocidadCoroutine != null)
            {
                StopCoroutine(debufoVelocidadCoroutine);
            }
            debufoVelocidadCoroutine = StartCoroutine(RemoverDebufoVelocidadDespuesDeTiempo(duracion));
        }
    }

    private IEnumerator RemoverDebufoVelocidadDespuesDeTiempo(float duracion)
    {
        yield return new WaitForSeconds(duracion);
        RemoverDebufoVelocidad();
    }

    public void RemoverDebufoVelocidad()
    {
        if (debufoVelocidadAplicado)
        {
            //_velocidadMovimiento = stats.velocidadMovimiento;
            velocidadActual =_velocidadMovimiento;
            debufoVelocidadAplicado = false;
        }
    }

    private void OnDestroy()
    {
        playerInputActions.Player.AtaqueCargado.started -= OnAttackChargedStarted;
        playerInputActions.Player.AtaqueCargado.canceled -= OnAttackChargedCanceled;
        playerInputActions.Player.Attack.started -= OnAttackStarted;
        playerInputActions.Player.Pausa.started -= OnPausePressed; // Desasignar la acción de pausa
        playerInputActions.Player.ArmaAnterior.started -= OnArmaAnterior; // Desasignar la acción de cambiar al arma anterior
        playerInputActions.Player.ArmaSiguiente.started -= OnArmaSiguiente; // Desasignar la acción de cambiar al arma siguiente
        playerInputActions.Disable(); // Asegúrate de deshabilitar los controles aquí
    }

    private void OnAttackChargedStarted(InputAction.CallbackContext context)
    {
        if (!juegoPausado && vidaActual > 0)
        {
            IniciarCarga();
        }
    }

    private void OnAttackChargedCanceled(InputAction.CallbackContext context)
    {
        if (!juegoPausado && vidaActual > 0)
        {
            CancelarCarga();
        }
    }

    private void OnAttackStarted(InputAction.CallbackContext context)
    {
        ActivarAtaque();
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (juegoPausado)
        {
            ReanudarJuego();
        }
        else
        {
            PausarJuego();
        }
    }

    private void OnArmaAnterior(InputAction.CallbackContext context)
    {
        if (!juegoPausado && vidaActual>0)
        {
            CambiarArmaAnterior();
        }
    }

    private void OnArmaSiguiente(InputAction.CallbackContext context)
    {
        if (!juegoPausado && vidaActual > 0)
        {
            CambiarArmaSiguiente();
        }
    }

    private void ActualizarArmaCollider()
    {
        if (armas[armaActual] != null)
        {
            armaCollider = armas[armaActual].GetComponent<Collider>();
        }
    }
    public void CambiarArmaAnterior()
    {
        armas[armaActual].gameObject.SetActive(false);
        armaActual = (armaActual - 1 + armas.Length) % armas.Length;
        armas[armaActual].gameObject.SetActive(true);
        ActualizarUIArmas();
        ActualizarArmaCollider(); // Actualizar el Collider del arma actual
    }

    public void CambiarArmaSiguiente()
    {
        armas[armaActual].gameObject.SetActive(false);
        armaActual = (armaActual + 1) % armas.Length;
        armas[armaActual].gameObject.SetActive(true);
        ActualizarUIArmas();
        ActualizarArmaCollider(); // Actualizar el Collider del arma actual
    }

    private void ActualizarUIArmas()
    {
        for (int i = 0; i < imagenesArmas.Length; i++)
        {
            if (i == armaActual)
            {
                imagenesArmas[i].color = Color.white; // Resaltar el arma seleccionada
                imagenesArmas[i].transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); // Aumentar el tamaño del arma seleccionada
            }
            else
            {
                imagenesArmas[i].color = Color.gray; // Atenuar las armas no seleccionadas
                imagenesArmas[i].transform.localScale = new Vector3(1f, 1f, 1f); // Restablecer el tamaño de las armas no seleccionadas
            }
        }
    }

    public void ReducirVida(float cantidad)
    {
        if (invulnerable) return; // Si el jugador es invulnerable, no recibir daño

        vidaActual -= cantidad;
        int nuevosCorazones = Mathf.CeilToInt(vidaActual / valorCorazon);

        if (nuevosCorazones < corazonesActuales)
        {
            corazonesActuales = nuevosCorazones;
            OnVidaCambiada?.Invoke(corazonesActuales);
        }

        if (vidaActual <= 0)
        {
            // Manejar la muerte del jugador
            derrota.SetActive(true);
            // No llamar a PausarJuego aquí
            Time.timeScale = 0f; // Pausar el juego sin mostrar el panel de pausa
        }
        else
        {
            StartCoroutine(InvulnerabilidadTemporal()); // Iniciar la corrutina de invulnerabilidad temporal
        }
    }

    public void ActualizarBarraDeVida()
    {
        // Calcula la cantidad de corazones actuales según la vida actual
        int nuevosCorazones = Mathf.CeilToInt(vidaActual / 10f); // 10f es el valor de cada corazón

        // Notifica a la UI para actualizar los corazones
        OnVidaCambiada?.Invoke(nuevosCorazones);
    }
    private IEnumerator InvulnerabilidadTemporal()
    {
        invulnerable = true;
        foreach (Renderer rend in renderers)
        {
            if (!renderersNoAfectados.Contains(rend))
            {
                rend.material = materialDaño; // Cambiar el material al recibir daño
            }
        }

        yield return new WaitForSeconds(0.5f); // Esperar medio segundo

        foreach (Renderer rend in renderers)
        {
            if (!renderersNoAfectados.Contains(rend))
            {
                rend.material = materialNormal; // Restaurar el material normal
            }
        }
        invulnerable = false;
    }

    public void ReducirResistenciaEscudo(float cantidad)
    {
        resistenciaEscudoActual -= cantidad;
        if (resistenciaEscudoActual < 0)
        {
            resistenciaEscudoActual = 0;
        }

        if (barraResistenciaEscudo != null)
        {
            barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo;
        }

        if (resistenciaEscudoActual <= 0)
        {
            // Manejar la ruptura del escudo
            escudoActivo = false;
        }

        // Reproducir el sonido del golpe en el escudo
        if (audioSource != null && sonidoGolpeEscudo != null)
        {
            audioSource.PlayOneShot(sonidoGolpeEscudo);
        }

        // Reiniciar la corrutina de regeneración del escudo
        if (regeneracionEscudoCoroutine != null)
        {
            StopCoroutine(regeneracionEscudoCoroutine);
        }
        regeneracionEscudoCoroutine = StartCoroutine(RegenerarResistenciaEscudo());

        // Reiniciar el temporizador de último daño
        tiempoUltimoDaño = 0f;
    }

    private IEnumerator RegenerarResistenciaEscudo()
    {
        // Esperar el tiempo de regeneración
        yield return new WaitForSeconds(tiempoRegeneracion);

        // Regenerar la resistencia del escudo
        while (resistenciaEscudoActual < stats.resistenciaEscudo && vidaActual > 0)
        {
            resistenciaEscudoActual += velocidadRegeneracion * Time.deltaTime;
            if (resistenciaEscudoActual > stats.resistenciaEscudo)
            {
                resistenciaEscudoActual = stats.resistenciaEscudo;
            }

            if (barraResistenciaEscudo != null)
            {
                barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo;
            }

            //if (resistenciaEscudoActual >= valorMinimoEscudo)
            //{
            //    escudoActivo = true;
            //}

            yield return null;
        }
    }

    public void IncrementarAtaqueTemporal(float cantidad, int golpes)
    {
        incrementoAtaqueTemporal = cantidad;
        golpesRestantes = golpes;
        uiManager.MostrarIncrementoAtaque(cantidad, golpes); // Actualizar la UI
    }

    public void PausarJuego()
    {
        if(vidaActual > 0)
        {
            Time.timeScale = 0f; // Pausar el juego
            juegoPausado = true;
            panelPausa.SetActive(true); // Mostrar el panel de pausa
        }

    }

    public void ReanudarJuego()
    {
        Time.timeScale = 1f; // Reanudar el juego
        juegoPausado = false;
        panelPausa.SetActive(false); // Ocultar el panel de pausa
    }

    public abstract void ActivarAtaque();


    public abstract void ActivarInteraction();

    protected void AtaqueCargadoArea()
    {
        if (incrementoAtaqueTemporal > 0 && vidaActual > 0)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, areaRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemigo"))
                {
                    Enemigo enemigo = hitCollider.GetComponent<Enemigo>();
                    if (enemigo != null)
                    {
                        enemigo.RecibirDanio(ataque + incrementoAtaqueTemporal); // Usar el incremento de ataque temporal
                        EmpujarEnemigo(enemigo);
                    }
                }
            }
            // Decrementar los golpes restantes después de un ataque cargado
            golpesRestantes--;
            uiManager.UsarGolpe();
            if (golpesRestantes <= 0)
            {
                incrementoAtaqueTemporal = 0; // Restablecer el incremento de ataque temporal
            }
        }
        else
        {
            uiManager.ActivarTemporalmente(Equis, 1f); // Mostrar el objeto "Equis" durante 1 segundo
            //uiManager.CambiarColorAtaqueImage(Color.blue, 1f); // Cambiar el color de la imagen a azul por un segundo
        }
    }

    public void IniciarCarga()
    {
        if (incrementoAtaqueTemporal > 0 && vidaActual > 0)
        {
            isCharging = true;
            chargeTime = 0f;
            if (ataqueCargadoEfecto != null)
            {
                ataqueCargadoEfecto.SetActive(true); // Mostrar el efecto del ataque cargado
            }
        }
        else
        {
            uiManager.ActivarTemporalmente(Equis, 1f); // Mostrar el objeto "Equis" durante 1 segundo
            //uiManager.CambiarColorAtaqueImage(Color.blue, 1f); // Cambiar el color de la imagen a azul por un segundo
        }
    }

    public void CancelarCarga()
    {
        isCharging = false;
        chargeTime = 0f;
        if (cargaBarra != null)
        {
            cargaBarra.fillAmount = 0f;
        }
        if (ataqueCargadoEfecto != null)
        {
            ataqueCargadoEfecto.SetActive(false); // Ocultar el efecto del ataque cargado
        }
    }

    public bool PuedeUsarEscudo()
    {
        return escudoActivo && resistenciaEscudoActual >= valorMinimoEscudo;
    }

    private void EmpujarEnemigo(Enemigo enemigo)
    {
        Rigidbody rb = enemigo.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = (enemigo.transform.position - transform.position).normalized;
            rb.AddForce(direction * pushForce, ForceMode.Impulse);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Dibujar el área de daño en el editor cuando el objeto está seleccionado
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, areaRadius);
    }

    public void Mover(Vector3 direccion)
    {
        // Normalizar la dirección para evitar que las velocidades se sumen
        if (direccion.magnitude > 1)
        {
            direccion.Normalize();
        }

        // Rotar la dirección del movimiento 45 grados
        Quaternion rotacion = Quaternion.Euler(0, 45, 0);
        Vector3 direccionRotada = rotacion * direccion;

        // Mover al jugador en la dirección rotada sin importar la dirección a la que está mirando
        Vector3 movimiento = new Vector3(direccionRotada.x, 0, direccionRotada.z) * velocidadActual * Time.deltaTime;
        transform.Translate(movimiento, Space.World);
    }

    public void AplicarBuffVelocidad(float incremento, float duracion)
    {
        StartCoroutine(BuffVelocidadCoroutine(incremento, duracion));
    }
    public void BarrVelocidad()
    {
        StartCoroutine(BarraVelocidad());
    }

    [SerializeField] public Image velocidadEffectBar; // Nueva barra para el efecto
    [SerializeField] private float duracionEfecto = 5f; // Duración del efecto de velocidad en segundos
    private IEnumerator BuffVelocidadCoroutine(float incremento, float duracion)
    {

        velocidadActual += incremento;
        yield return new WaitForSeconds(duracion);
        velocidadActual -= incremento;
    }

    private IEnumerator BarraVelocidad()
    {
        velocidadEffectBar.gameObject.SetActive(true);
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionEfecto)
        {
            tiempoTranscurrido += Time.deltaTime;
            velocidadEffectBar.fillAmount = 1f - (tiempoTranscurrido / duracionEfecto);
            yield return null;
        }
        velocidadEffectBar.fillAmount = 0f;
        velocidadEffectBar.gameObject.SetActive(false);
    }

}
