using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AderezoVelocidad : Aderezo
{
    public float incrementoVelocidad = 2f; // Incremento de velocidad de movimiento
    public Material materialVelocidadEnemigo; // Asigna este material desde el Inspector

    private Dictionary<Renderer, Material> materialesOriginales = new Dictionary<Renderer, Material>();

    protected override void Awake()
    {
        base.Awake();
        tieneDuracion = true;
        // duracionEfecto = 5f;
    }
    protected override void Update()
    {
        base.Update();
    }
    protected override void IncrementarVelocidadJugador()
    {
        if (jugador != null)
        {
            jugador.AplicarBuffVelocidad(incrementoVelocidad, duracionEfecto, this);
            jugador.BarrVelocidad(); // Actualizar la UI de velocidad del jugador

            // Opcional: Mover el aderezo lejos para evitar interacción 
            transform.position = new Vector3(10000, 10000, 10000);

            Debug.Log("Velocidad del jugador incrementada temporalmente");
        }
    }

    protected override void IncrementarVelocidadEnemigo()
    {

            if (enemigo != null && !enemigo.yaTomoAderezo)
            {
                enemigo.yaTomoAderezo = true;
                // Guardar materiales originales y aplicar el nuevo material
                if (materialVelocidadEnemigo != null)
            {
                materialesOriginales.Clear();
                foreach (var rend in enemigo.GetComponentsInChildren<Renderer>())
                {
                    if (rend.GetComponent<ParticleSystem>() == null)
                    {
                        materialesOriginales[rend] = rend.material;
                        rend.material = materialVelocidadEnemigo;
                    }
                }
            }
        
            StartCoroutine(AplicarIncrementoVelocidadEnemigo());
            gameObject.SetActive(false); // Desactivar el objeto instanciado
            Debug.Log("Velocidad del enemigo incrementada temporalmente");
        }
    }

    private IEnumerator AplicarIncrementoVelocidadEnemigo()
    {
        if (enemigo != null)
        {
            enemigo.velocidadMovimientoInicial += incrementoVelocidad;
            yield return new WaitForSeconds(duracionEfecto);
            enemigo.velocidadMovimientoInicial -= incrementoVelocidad;

            // Restaurar materiales originales
            foreach (var kvp in materialesOriginales)
            {
                if (kvp.Key != null)
                    kvp.Key.material = kvp.Value;
            }
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