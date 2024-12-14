using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        // Asegurarse de que solo haya una instancia de GameManager
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReiniciarJuego()
    {
        // Reiniciar el tiempo de juego
        Time.timeScale = 1f;
        // Cargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}










