using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked;
    public Animator animator;
    public AudioClip sonidoCerrar; // Clip de audio para el sonido de cerrar
    public AudioClip sonidoAbrir; // Clip de audio para el sonido de abrir
    private AudioSource audioSource; // Referencia al componente AudioSource
    private bool isInitialized = false; // Variable para controlar si la puerta ha sido inicializada

    private void Start()
    {
        // Obtener el AudioSource de la cámara principal
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            audioSource = mainCamera.GetComponent<AudioSource>();
        }

        if (isLocked)
        {
            Lock();
        }
        else
        {
            Unlock(false); // No reproducir sonido al inicio
        }

        isInitialized = true; // Marcar la puerta como inicializada
    }

    public void Lock()
    {
        isLocked = true;
        if (animator != null)
        {
            animator.SetBool("isLocked", true);
        }
        if (audioSource != null && sonidoCerrar != null)
        {
            audioSource.PlayOneShot(sonidoCerrar); // Reproducir el sonido de cerrar
        }
        gameObject.SetActive(true); // Activar la puerta
    }

    public void Unlock(bool playSound = true)
    {
        isLocked = false;
        if (animator != null)
        {
            animator.SetBool("isLocked", false);
        }
        if (playSound && isInitialized && audioSource != null && sonidoAbrir != null)
        {
            audioSource.PlayOneShot(sonidoAbrir); // Reproducir el sonido de abrir
        }
        gameObject.SetActive(false); // Desactivar la puerta
    }
}

