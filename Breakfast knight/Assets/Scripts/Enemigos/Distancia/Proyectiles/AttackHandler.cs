using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackHandler : MonoBehaviour
{
    public AtaqueEnemigo[] ataques;
    public AtaqueEnemigo ataqueActual;
    private int attackCount = 0;
    public float cooldownTime = 1f; // Tiempo de cooldown en segundos
    private float lastAttackTime;

    private void Start()
    {
        ataqueActual = ataques[0];
        lastAttackTime = -cooldownTime; // Permitir atacar inmediatamente al inicio
    }

    public void ActivarAtaque()
    {
        if (Time.time >= lastAttackTime + cooldownTime)
        {
            ataqueActual.Atacar();
            lastAttackTime = Time.time;
            attackCount++;

            if (attackCount >= 3)
            {
                CambiarAtaque();
                attackCount = 0;
            }
        }
    }

    private void CambiarAtaque()
    {
        int currentIndex = Array.IndexOf(ataques, ataqueActual);
        int nextIndex = (currentIndex + 1) % ataques.Length;
        ataqueActual = ataques[nextIndex];
    }
}


