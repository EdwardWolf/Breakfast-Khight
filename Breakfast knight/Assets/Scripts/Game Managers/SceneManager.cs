using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    // M�todo para cambiar a la escena del juego
    public void CambiarAEscenaDelJuego()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Principal");
    }

    // M�todo para cambiar a la pantalla de cr�ditos
    public void CambiarAPantallaDeCreditos()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Creditos");
    }
    // M�todo para cambiar a la pantalla de menu
    public void CambiarAPantallaDeMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Inicio");
    }

    // M�todo para salir del juego
    public void SalirDelJuego()
    {
        Application.Quit();
    }
}
