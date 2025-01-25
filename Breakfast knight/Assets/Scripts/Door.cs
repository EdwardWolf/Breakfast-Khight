using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool isLocked = true; // Indica si la puerta está cerrada
    public Animator animator; // Referencia al componente Animator
    public Material materialBase; // Material base

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Lock()
    {
        isLocked = true;
        //animator.SetTrigger("Lock"); // Activar la animación de bloqueo
        Debug.Log("Puerta bloqueada");
        GetComponent<Collider>().isTrigger = false; // Desactivar el trigger para bloquear el paso
        materialBase.color = Color.red;
    }

    public void Unlock()
    {
        isLocked = false;
        //animator.SetTrigger("Unlock"); // Activar la animación de desbloqueo
        Debug.Log("Puerta desbloqueada");
        GetComponent<Collider>().isTrigger = true; // Activar el trigger para permitir el paso
        materialBase.color = Color.white;
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
        //animator.SetTrigger("Open"); // Activar la animación de apertura
        Debug.Log("Puerta abierta");
    }
}

