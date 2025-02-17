using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public Transform target; // El objeto que la c�mara va a seguir (el personaje)
    public Vector3 offset = new Vector3(10, 10, -10); // Desfase de la c�mara respecto al personaje

    void LateUpdate()
    {
        // Mover la c�mara a la posici�n del target m�s el offset
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }
}
