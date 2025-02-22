using UnityEngine;

public class Tenedor : Arma
{
    [SerializeField] private ArmaData armaData;
    private Animator animator;

    private void Awake()
    {
        if (armaData != null)
        {
            nombreArma = armaData.nombreArma;
            daño = armaData.daño;
            velocidadDeAtaque = armaData.velocidadDeAtaque;
        }
        animator = GetComponent<Animator>();
    }

    public override void Atacar()
    {
        // Implementación del ataque del tenedor
        Debug.Log(nombreArma + " atacando con " + daño + " de daño.");
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }
    }

    public override void Pasiva()
    {
        // Implementación de la habilidad pasiva del tenedor
        Debug.Log("Habilidad pasiva de " + nombreArma + " activada.");
    }

    public override void Activa()
    {
        // Implementación de la habilidad activa del tenedor
        Debug.Log("Habilidad activa de " + nombreArma + " activada.");
    }
}
