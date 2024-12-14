using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Jugador : MonoBehaviour
{
    [SerializeField] protected Stats stats;
    [SerializeField] protected float vidaActual;
    [SerializeField] protected float resistenciaEscudoActual;
    [SerializeField] protected float _velocidadMovimiento;
    [SerializeField] protected float velocidadAtaque;
    [SerializeField] public float ataque; // Añadir una variable para el ataque
    private int golpesRestantes; // Variable para almacenar los golpes restantes con el ataque incrementado
    [SerializeField] private Material materialBase; // Material base
    [SerializeField] private Material materialAumento; // Material de aumento de ataque
    public Renderer renderer; // Referencia al Renderer del objeto
    public GameObject derrota; // Referencia al objeto de derrota
    public float areaRadius = 5f; // Radio del área de efecto del ataque cargado
    public float pushForce = 10f; // Fuerza de empuje del ataque cargado
    public Image cargaBarra; // Referencia a la barra de carga
    public Image barraResistenciaEscudo; // Referencia a la barra de resistencia del escudo
    private bool isCharging = false;
    private float chargeTime = 0f;
    private float maxChargeTime = 2f; // Tiempo máximo de carga
    private bool escudoActivo = false;
    private float tiempoUltimoDaño = 0f;
    private float tiempoRegeneracion = 5f; // Tiempo de espera para comenzar la regeneración
    private float velocidadRegeneracion = 5f; // Velocidad de regeneración de la resistencia del escudo
    private float valorMinimoEscudo = 10f; // Valor mínimo de resistencia del escudo para poder usarlo

    private int corazonesActuales;

    public delegate void VidaCambiada(int corazones);
    public static event VidaCambiada OnVidaCambiada;

    protected virtual void Start()
    {
        // Asignar los valores iniciales de las estadísticas.
        vidaActual = stats.vida;
        resistenciaEscudoActual = stats.resistenciaEscudo;
        _velocidadMovimiento = stats.velocidadMovimiento;
        velocidadAtaque = stats.velocidadAtaque;
        ataque = stats.ataque; // Inicializar el ataque
        corazonesActuales = Mathf.CeilToInt(vidaActual / 30f);
        OnVidaCambiada?.Invoke(corazonesActuales);
        derrota.SetActive(false);

        if (renderer != null)
        {
            renderer.material = materialBase; // Asignar el material base al inicio
        }

        if (cargaBarra != null)
        {
            cargaBarra.fillAmount = 0f; // Inicializar la barra de carga
        }

        if (barraResistenciaEscudo != null)
        {
            barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo; // Inicializar la barra de resistencia del escudo
        }
    }

    public void ReducirVida(float cantidad)
    {
        vidaActual -= cantidad;
        int nuevosCorazones = Mathf.CeilToInt(vidaActual / 30f);

        if (nuevosCorazones < corazonesActuales)
        {
            corazonesActuales = nuevosCorazones;
            OnVidaCambiada?.Invoke(corazonesActuales);
        }

        if (vidaActual <= 0)
        {
            // Manejar la muerte del jugador
            Debug.Log("Jugador ha muerto");
            derrota.SetActive(true);
            PausarJuego();
        }
    }

    public void ReducirResistenciaEscudo(float cantidad)
    {
        resistenciaEscudoActual -= cantidad;
        if (resistenciaEscudoActual < 0)
        {
            resistenciaEscudoActual = 0;
        }
        tiempoUltimoDaño = Time.time;

        if (barraResistenciaEscudo != null)
        {
            barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo;
        }

        if (resistenciaEscudoActual <= 0)
        {
            // Manejar la ruptura del escudo
            Debug.Log("Escudo roto");
            escudoActivo = false;
        }
    }

    private void RegenerarResistenciaEscudo()
    {
        if (Time.time - tiempoUltimoDaño >= tiempoRegeneracion && resistenciaEscudoActual < stats.resistenciaEscudo)
        {
            Debug.Log("Esta regenerando escudo");
            resistenciaEscudoActual += velocidadRegeneracion * Time.deltaTime;
            if (resistenciaEscudoActual > stats.resistenciaEscudo)
            {
                resistenciaEscudoActual = stats.resistenciaEscudo;
            }

            if (barraResistenciaEscudo != null)
            {
                barraResistenciaEscudo.fillAmount = resistenciaEscudoActual / stats.resistenciaEscudo;
            }

            if (resistenciaEscudoActual >= valorMinimoEscudo)
            {
                escudoActivo = true;
            }
        }
    }

    public void IncrementarAtaque(float cantidad)
    {
        ataque += cantidad;
        Debug.Log("Ataque incrementado en " + cantidad);
    }

    public void IncrementarAtaqueTemporal(float cantidad, int golpes)
    {
        ataque += cantidad;
        golpesRestantes = golpes;
        Debug.Log("Ataque incrementado temporalmente en " + cantidad + " por " + golpes + " golpes");
        CambiarMaterial(materialAumento); // Cambiar al material de aumento de ataque
    }

    public void AsestarGolpe()
    {
        if (golpesRestantes > 0)
        {
            golpesRestantes--;
            if (golpesRestantes <= 0)
            {
                ataque = stats.ataque;
                Debug.Log("Ataque restablecido a " + stats.ataque);
                CambiarMaterial(materialBase); // Cambiar al material base
            }
        }
    }

    private void CambiarMaterial(Material nuevoMaterial)
    {
        if (renderer != null)
        {
            renderer.material = nuevoMaterial;
        }
    }

    private void PausarJuego()
    {
        Time.timeScale = 0f; // Pausar el juego
        // Opcional: Mostrar un mensaje de "Game Over" o una pantalla de pausa
    }

    public abstract void Mover(Vector3 direccion);
    public abstract void ActivarAtaque();
    public abstract void ActivarAtaqueCargado();
    public abstract void ActivarInteraction();

    protected void AtaqueCargadoArea()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, areaRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemigo"))
            {
                Enemigo enemigo = hitCollider.GetComponent<Enemigo>();
                if (enemigo != null)
                {
                    enemigo.RecibirDanio(ataque);
                    EmpujarEnemigo(enemigo);
                }
            }
        }
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

    protected void Update()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            if (cargaBarra != null)
            {
                cargaBarra.fillAmount = chargeTime / maxChargeTime;
            }

            if (chargeTime >= maxChargeTime)
            {
                ActivarAtaqueCargado();
                isCharging = false;
                chargeTime = 0f;
                if (cargaBarra != null)
                {
                    cargaBarra.fillAmount = 0f;
                }
            }
        }

        RegenerarResistenciaEscudo();
    }

    public void IniciarCarga()
    {
        isCharging = true;
        chargeTime = 0f;
        
    }

    public void CancelarCarga()
    {
        isCharging = false;
        chargeTime = 0f;
        if (cargaBarra != null)
        {
            cargaBarra.fillAmount = 0f;
        }
    }

    public bool PuedeUsarEscudo()
    {
        return escudoActivo && resistenciaEscudoActual >= valorMinimoEscudo;
    }
}