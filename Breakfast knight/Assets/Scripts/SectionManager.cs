using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{

    public Door[] doors; // Puertas que se bloquearán y desbloquearán
    public int enemiesToDefeat = 5; // Cantidad de enemigos a derrotar para desbloquear las puertas
    public int enemiesDefeated = 0; // Contador de enemigos derrotados
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LockDoors();
            
        }
    }

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        if (enemiesDefeated >= enemiesToDefeat)
        {
            UnlockDoors();
        }
    }

    private void LockDoors()
    {
        foreach (Door door in doors)
        {

            door.Lock();

        }
    }

    private void UnlockDoors()
    {
        foreach (Door door in doors)
        {
            door.Unlock();

        }
    }


}
