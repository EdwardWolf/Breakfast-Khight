using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AERecto : AtaqueEnemigo
{
        public Transform jugador;

        void Update()
        {
         Vector3 direction = (jugador.position - transform.position).normalized;
         Disparar(direction);

        }

    
}
