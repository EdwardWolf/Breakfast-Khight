using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    // Método para cambiar a la escena del juego
    public void CambiarAEscenaDelJuego()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Principal");
    }

    // Método para cambiar a la pantalla de créditos
    public void CambiarAPantallaDeCreditos()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Creditos");
    }
    // Método para cambiar a la pantalla de menu
    public void CambiarAPantallaDeMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Inicio");
    }

    // Método para salir del juego
    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
