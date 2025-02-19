using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<SectionManager> sections; // Lista de secciones en el nivel

    public void AddSection(SectionManager section)
    {
        if (!sections.Contains(section))
        {
            sections.Add(section);
        }
    }


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
        UnityEngine.SceneManagement.SceneManager.LoadScene("Principal");

    }

    public void PausarJuego()
    {
        // Reiniciar el tiempo de juego
        Time.timeScale = 0f;


    }


    public List<Door> doors; // Lista de puertas en el nivel
    public List<Key> keys; // Lista de llaves en el nivel

    private void Start()
    {
        // Inicializar puertas y llaves si es necesario
        // Inicializar secciones si es necesario
    }

    public void AddDoor(Door door)
    {
        if (!doors.Contains(door))
        {
            doors.Add(door);
        }
    }

    public void AddKey(Key key)
    {
        if (!keys.Contains(key))
        {
            keys.Add(key);
        }
    }

    public void UnlockDoor(Door door)
    {
        if (doors.Contains(door))
        {
            door.Unlock();
        }
    }
}











