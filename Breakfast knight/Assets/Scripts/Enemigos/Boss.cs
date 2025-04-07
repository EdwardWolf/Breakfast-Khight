using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemigo
{
    public float maxHealth = 1000f;
    private float currentHealth;
    public BossHealthBar bossHealthBar; // Referencia al script de la barra de vida

    protected override void Start()
    {
        base.Start();
        currentHealth = maxHealth;
        bossHealthBar.Initialize("Nombre del Jefe", maxHealth);
    }

    public void ReceiveDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }
        bossHealthBar.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Lógica para manejar la muerte del jefe
        Debug.Log("El jefe ha muerto");
    }
}
