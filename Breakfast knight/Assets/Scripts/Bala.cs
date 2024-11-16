using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bala : MonoBehaviour
{
        public float speed = 10f;

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

    //void OnCollisionEnter(Collision collision)
    //{

    //    Debug.Log("Hubo colision");
    //    gameObject.SetActive(false);

    //}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
        Debug.Log("Golpeado");
            gameObject.SetActive(false);
        }
        if (other.CompareTag("Muro"))
        {
            Debug.Log("Pego Muro");
            gameObject.SetActive(false);
        }
    }

}
