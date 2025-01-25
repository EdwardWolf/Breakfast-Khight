using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public Door door; // Referencia a la puerta que esta llave puede abrir

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.Unlock();
            Destroy(gameObject); // Destruir la llave después de recogerla
            Debug.Log("Llave recogida");
        }
    }
}
