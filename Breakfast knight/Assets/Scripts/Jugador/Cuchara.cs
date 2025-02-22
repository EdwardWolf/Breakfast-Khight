using UnityEngine;

public class Cuchara : Arma
{
    [SerializeField] private ArmaData armaData;
    private Animator animator;

    private void Awake()
    {
        if (armaData != null)
        {
            nombreArma = armaData.nombreArma;
            da�o = armaData.da�o;
            velocidadDeAtaque = armaData.velocidadDeAtaque;
        }
        animator = GetComponent<Animator>();
    }

    public override void Atacar()
    {
        // Implementaci�n del ataque de la cuchara
        Debug.Log(nombreArma + " atacando con " + da�o + " de da�o.");
        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }
    }

    public override void Pasiva()
    {
        // Implementaci�n de la habilidad pasiva de la cuchara
        Debug.Log("Habilidad pasiva de " + nombreArma + " activada.");
    }

    public override void Activa()
    {
        // Implementaci�n de la habilidad activa de la cuchara
        Debug.Log("Habilidad activa de " + nombreArma + " activada.");
    }
}
