using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraFollow : MonoBehaviour
{
    public FadeObjects _fader;
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

    void Update()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Calcular la dirección desde la cámara hacia el jugador
            Vector3 dir = player.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir.normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == null)
                    return;

                // Si el objeto golpeado es el jugador, desactivar el fade
                if (hit.collider.gameObject == player)
                {
                    if (_fader != null)
                    {
                        _fader.DoFade = false;
                        _fader = null;
                    }
                }
                else
                {
                    // Solo aplicar fade si el objeto tiene el script FadeObjects
                    FadeObjects fadeObj = hit.collider.gameObject.GetComponent<FadeObjects>();
                    if (fadeObj != null)
                    {
                        if (_fader != null && _fader != fadeObj)
                        {
                            _fader.DoFade = false;
                        }
                        _fader = fadeObj;
                        _fader.DoFade = true;
                    }
                    else
                    {
                        if (_fader != null)
                        {
                            _fader.DoFade = false;
                            _fader = null;
                        }
                    }
                }
            }
            else
            {
                if (_fader != null)
                {
                    _fader.DoFade = false;
                    _fader = null;
                }
            }
        }
    }
}