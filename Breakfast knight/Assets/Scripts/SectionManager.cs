using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionManager : MonoBehaviour
{
    public Door[] doors; // Puertas que se bloquearán y desbloquearán

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LockDoors();
        }
    }

    private void Update()
    {
        if (AreAllChildrenDeactivated())
        {
            UnlockDoors();
        }
    }

    private bool AreAllChildrenDeactivated()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
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

