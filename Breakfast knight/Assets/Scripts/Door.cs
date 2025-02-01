using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked; // Indica si la puerta está cerrada
    public Animator animator; // Referencia al componente Animator
    public Material materialBase; // Material base
    public Material lockedMaterial; // Material cuando la puerta está cerrada
    public Material unlockedMaterial; // Material cuando la puerta está abierta
    public Collider detectionCollider; // Collider para el área de detección

    private void Start()
    {
        GetComponent<Collider>();
        isLocked = false;
        if (materialBase != null)
        {
            GetComponent<Renderer>().material = materialBase; // Asignar el material base al inicio
        }
    }

    public void Lock()
    {
        isLocked = true;
        //animator.SetTrigger("Lock"); // Activar la animación de bloqueo
        Debug.Log("Puerta bloqueada");
        //GetComponent<Collider>().enabled = true; // Desactivar el trigger para bloquear el paso
        GetComponent<Collider>().isTrigger = false; // Desactivar el trigger para bloquear el paso

        if (lockedMaterial != null)
        {
            GetComponent<Renderer>().material = lockedMaterial; // Cambiar al material de puerta cerrada
        }
    }

    public void Unlock()
    {
        isLocked = false;
        //animator.SetTrigger("Unlock"); // Activar la animación de desbloqueo
        Debug.Log("Puerta desbloqueada");
        //GetComponent<Collider>().enabled = false; // Activar el trigger para permitir el paso
        GetComponent<Collider>().isTrigger = true; // Desactivar el trigger para bloquear el paso

        if (unlockedMaterial != null)
        {
            GetComponent<Renderer>().material = unlockedMaterial; // Cambiar al material de puerta abierta
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isLocked)
        {
            OpenDoor();
        }
    }

    private void OpenDoor()
    {
        if (animator != null)
        {
            animator.SetTrigger("Open"); // Activar la animación de apertura
        }
        Debug.Log("Puerta abierta");
    }
}
