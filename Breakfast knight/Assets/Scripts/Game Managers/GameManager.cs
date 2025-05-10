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
    public GameObject bossEnemy; // Referencia al enemigo jefe
    public Image bossHealthBar; // Referencia al script de la barra de vida del jefe
    public string bossName = "Jefe Final"; // Nombre del jefe
    public float bossMaxHealth = 100f; // Vida máxima del jefe
    public TextMeshProUGUI bossNameText; // Referencia al componente TextMeshProUGUI


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

    private void Start()
    {
        ResetBossState();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento de carga de escena
    

}

    public void ReiniciarJuego()
    {
        // Reiniciar el tiempo de juego
        Time.timeScale = 1f;

        // Reiniciar el estado del jefe
        ResetBossState();

        // Cargar la escena actual
        UnityEngine.SceneManagement.SceneManager.LoadScene("Principal");
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetBossState(); // Reiniciar el estado del jefe al cargar la escena
    }
    private void ResetBossState()
    {
        if (bossEnemy != null) bossEnemy.SetActive(false);
        if (bossHealthBar != null) bossHealthBar.gameObject.SetActive(false);
        if (bossNameText != null) bossNameText.gameObject.SetActive(false);
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

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto que entra es el jugador
        if (other.CompareTag("Player"))
        {
            ActivateBoss();
        }
    }

    private void ActivateBoss()
    {
        if (bossEnemy != null)
        {
            bossEnemy.SetActive(true); // Activar el jefe
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true); // Activar la barra de vida
            StartCoroutine(FillHealthBarGradually()); // Llenar la barra de vida gradualmente
        }

        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(true); // Mostrar el texto
            bossNameText.text = bossName; // Asignar el nombre del jefe
        }
    }

    private IEnumerator FillHealthBarGradually()
    {
        float fillSpeed = 0.3f; // Velocidad de llenado (ajusta este valor según lo que necesites)
        float currentFill = 0f;

        while (currentFill < 1f)
        {
            currentFill += Time.deltaTime * fillSpeed; // Incrementar el valor de llenado
            bossHealthBar.fillAmount = Mathf.Clamp01(currentFill); // Actualizar la barra de vida
            yield return null; // Esperar al siguiente frame
        }
    }
    private void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse del evento al destruir el GameManager
    }
}
