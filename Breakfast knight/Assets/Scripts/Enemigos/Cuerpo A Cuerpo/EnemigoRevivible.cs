using System.Collections;
using UnityEngine;

public class EnemigoRevivible : Enemigo
{
    [Range(0, 1)]
    public float probabilidadDeRevivir = 0.5f; // Probabilidad de revivir (0 a 1)
    public float vidaAlRevivir = 50f; // Vida con la que revivir� el enemigo

    private new void Update()
    {
        if (vidaE <= 0)
        {
            StartCoroutine(IntentarRevivir());
        }
    }


    private IEnumerator IntentarRevivir()
    {
        yield return new WaitForSeconds(1f); // Esperar un segundo antes de intentar revivir

        if (Random.value <= probabilidadDeRevivir)
        {
            Revivir();
        }
        else
        {
            DesactivarEnemigo();
        }
    }

    private void Revivir()
    {
        vidaE = vidaAlRevivir;
        // Aqu� puedes agregar cualquier l�gica adicional para cuando el enemigo reviva
        // Por ejemplo, reproducir una animaci�n o sonido de resurrecci�n
        ActualizarBarraDeVida(); // Actualizar la barra de vida del enemigo
    }
}
