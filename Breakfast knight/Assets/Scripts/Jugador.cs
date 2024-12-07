using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class Jugador : MonoBehaviour
{
    [SerializeField] protected Stats stats;
    //public Stats stats;
    [SerializeField] protected float vidaActual;
    [SerializeField] protected float resistenciaEscudoActual;
    [SerializeField] protected float _velocidadMovimiento;
    [SerializeField] protected float velocidadAtaque;
    [SerializeField] protected Image Image;

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
        corazonesActuales = Mathf.CeilToInt(vidaActual / 30f);
        OnVidaCambiada?.Invoke(corazonesActuales);
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
            PausarJuego();
        }
    }
    private void PausarJuego()
    {
        Time.timeScale = 0f; // Pausar el juego
        Image.gameObject.SetActive(true); // Mostrar la imagen de pausa
    }

    public abstract void Mover(Vector3 direccion);
    public abstract void ActivarAtaque();
    public abstract void ActivarAtaqueCargado();
    public abstract void ActivarInteraction();
}
