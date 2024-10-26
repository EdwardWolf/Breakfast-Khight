using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target; // El objeto que la cámara va a seguir (el personaje)
    public Vector3 offset = new Vector3(10, 10, -10); // Desfase de la cámara respecto al personaje

    void LateUpdate()
    {
        // Mover la cámara a la posición del target más el offset
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
