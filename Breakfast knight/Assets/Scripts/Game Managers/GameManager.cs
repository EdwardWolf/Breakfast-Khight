using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<SectionManager> sections; // Lista de secciones en el nivel
    public List<EnemySpawner> enemySpawners; // Lista de spawners de enemigos en el nivel
    public List<Door> doors; // Lista de puertas en el nivel
    public List<Key> keys; // Lista de llaves en el nivel


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
        // Pausar el tiempo de juego
        Time.timeScale = 0f;
    }

    // Métodos para administrar SectionManager
    public void AddSection(SectionManager section)
    {
        if (!sections.Contains(section))
        {
            sections.Add(section);
        }
    }

    public void RemoveSection(SectionManager section)
    {
        if (sections.Contains(section))
        {
            sections.Remove(section);
        }
    }

    public void ActivateSection(SectionManager section)
    {
        section.gameObject.SetActive(true);
    }

    public void DeactivateSection(SectionManager section)
    {
        section.gameObject.SetActive(false);
    }

    // Métodos para administrar EnemySpawner
    public void AddEnemySpawner(EnemySpawner spawner)
    {
        if (!enemySpawners.Contains(spawner))
        {
            enemySpawners.Add(spawner);
        }
    }

    public void RemoveEnemySpawner(EnemySpawner spawner)
    {
        if (enemySpawners.Contains(spawner))
        {
            enemySpawners.Remove(spawner);
        }
    }

    public void ActivateEnemySpawner(EnemySpawner spawner)
    {
        spawner.gameObject.SetActive(true);
    }

    public void DeactivateEnemySpawner(EnemySpawner spawner)
    {
        spawner.gameObject.SetActive(false);
    }

    // Métodos para administrar Door
    public void AddDoor(Door door)
    {
        if (!doors.Contains(door))
        {
            doors.Add(door);
        }
    }

    public void RemoveDoor(Door door)
    {
        if (doors.Contains(door))
        {
            doors.Remove(door);
        }
    }

    public void LockDoor(Door door)
    {
        if (doors.Contains(door))
        {
            door.Lock();
        }
    }

    public void UnlockDoor(Door door)
    {
        if (doors.Contains(door))
        {
            door.Unlock();
        }
    }

    // Métodos para administrar Key
    public void AddKey(Key key)
    {
        if (!keys.Contains(key))
        {
            keys.Add(key);
        }
    }

    public void RemoveKey(Key key)
    {
        if (keys.Contains(key))
        {
            keys.Remove(key);
        }
    }

    public void ActivateKey(Key key)
    {
        key.gameObject.SetActive(true);
    }

    public void DeactivateKey(Key key)
    {
        key.gameObject.SetActive(false);
    }

}
