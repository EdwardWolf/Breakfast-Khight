using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image healthBar; // Referencia a la imagen de la barra de vida
    public TMP_Text bossNameText; // Referencia al texto del nombre del jefe
    private float maxHealth;
    private float currentHealth;

    // Método para inicializar la barra de vida del jefe
    public void Initialize(string bossName, float maxHealth)
    {
        bossNameText.text = bossName;
        this.maxHealth = maxHealth;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    // Método para actualizar la vida del jefe
    public void UpdateHealth(float health)
    {
        currentHealth = health;
        UpdateHealthBar();
    }

    // Método para actualizar la imagen de la barra de vida
    private void UpdateHealthBar()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}