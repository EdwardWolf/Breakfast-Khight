using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemigo
{
    [Header("Boss Settings")]
    public string bossName = "Jefe Final";
    public float maxHealth = 1000f;
    private float currentHealth;

    [Header("UI References")]
    public BossHealthBar bossHealthBar;
    public TextMeshProUGUI bossNameText;

    protected override void Start()
    {
        base.Start();
        currentHealth = maxHealth;

        // Inicializa la barra de vida y el nombre
        if (bossHealthBar != null)
        {
            bossHealthBar.Initialize(bossName, maxHealth);
            bossHealthBar.gameObject.SetActive(false); // Oculta la barra hasta que se active el jefe
        }
        if (bossNameText != null)
        {
            bossNameText.text = bossName;
            bossNameText.gameObject.SetActive(false);
        }
    }

    protected override void Update()
    {
        DetectPlayer();

        // Verificar si el collider del escudo está desactivado y limpiar la referencia
        if (escudoCollider != null && !escudoCollider.enabled)
        {
            enContactoConEscudo = false;
            escudoCollider = null;
            if (reducirResistenciaEscudoCoroutine != null)
            {
                StopCoroutine(reducirResistenciaEscudoCoroutine);
                reducirResistenciaEscudoCoroutine = null;
            }
        }
    }

    // Sobrescribe el método de daño para actualizar la barra de vida del jefe
    //public override void RecibirDanio(float cantidad)
    //{
    //    currentHealth -= cantidad;
    //    if (currentHealth < 0) currentHealth = 0;

    //    if (bossHealthBar != null)
    //        bossHealthBar.UpdateHealth(currentHealth);

    //    if (currentHealth <= 0)
    //        Die();
    //}

    private void Die()
    {
        Debug.Log("El jefe ha muerto");
        // Aquí puedes añadir lógica de muerte, animaciones, recompensas, etc.
        DesactivarEnemigo();
        if (bossHealthBar != null)
            bossHealthBar.gameObject.SetActive(false);
        if (bossNameText != null)
            bossNameText.gameObject.SetActive(false);
    }

    // Llama este método para activar la UI del jefe (por ejemplo, desde un trigger externo)
    public void ActivateBoss()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.gameObject.SetActive(true);
            StartCoroutine(FillHealthBarGradually());
        }
        if (bossNameText != null)
        {
            bossNameText.gameObject.SetActive(true);
            bossNameText.text = bossName;
        }
    }

    private IEnumerator FillHealthBarGradually()
    {
        float fillSpeed = 0.3f;
        float currentFill = 0f;
        while (currentFill < 1f)
        {
            currentFill += Time.deltaTime * fillSpeed;
            bossHealthBar.healthBar.fillAmount = Mathf.Clamp01(currentFill);
            yield return null;
        }
    }
}
