using System.Collections;
using UnityEngine;

public class AderezoAtaque : Aderezo
{
    public Material materialAtaqueEnemigo; // Asigna este material desde el Inspector
    protected override void Awake()
    {
        base.Awake();
        tieneDuracion = true;
        // duracionEfecto = 5f; // Puedes ajustar la duración si lo deseas
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void IncrementarAtaqueJugador()
    {
        if (jugador != null)
        {
            jugador.StartCoroutine(HabilitarAtaqueCargadoTemporal(jugador));
            jugador.BarrAtaqueCargado(duracionEfecto); // <-- Añade esta línea
                                                       // Mover el aderezo lejos para evitar interacción
            transform.position = new Vector3(10000, 10000, 10000);

            Debug.Log("Ataque cargado habilitado temporalmente");
        }
    }

    private IEnumerator HabilitarAtaqueCargadoTemporal(Jugador jugador)
    {
        jugador.puedeAtaqueCargado = true;

        // Mostrar en la UI que el ataque cargado está habilitado
        if (jugador.uiManager != null)
        {
            jugador.uiManager.MostrarIncrementoAtaque(0, duracionEfecto);
        }

        float tiempo = 0f;
        while (tiempo < duracionEfecto)
        {
            tiempo += Time.deltaTime;
            yield return null;
        }

        jugador.puedeAtaqueCargado = false;

        if (jugador.uiManager != null)
        {
            jugador.uiManager.OcultarIncrementoAtaque();
        }

        // Ocultar el aderezo al finalizar el efecto
        gameObject.SetActive(false);
    }

    protected override void IncrementarAtaqueEnemigo()
    {
        if (enemigo != null)
        {
            float incremento = 10f; // Ajusta el valor según el balance del juego
            enemigo.IncrementarAtaque(incremento);

            // Aplica el material solo a los renderers que NO tengan ParticleSystem
            if (materialAtaqueEnemigo != null)
            {
                foreach (var rend in enemigo.GetComponentsInChildren<Renderer>())
                {
                    if (rend.GetComponent<ParticleSystem>() == null)
                    {
                        rend.material = materialAtaqueEnemigo;
                    }
                }
            }

            Debug.Log($"El enemigo {enemigo.name} ha incrementado su ataque en {incremento}");

            transform.position = new Vector3(10000, 10000, 10000);
            gameObject.SetActive(false);
        }
    }

    public virtual void Recoger(Jugador jugador)
    {
        Destroy(gameObject);
    }

    public void OcultarAderezo()
    {
        gameObject.SetActive(false);
    }
}