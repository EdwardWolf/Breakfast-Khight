using UnityEngine;

public class Cuchara : Arma
{
    [SerializeField] private ArmaData armaData;

    private void Awake()
    {
        if (armaData != null)
        {
            nombreArma = armaData.nombreArma;
            daño = armaData.daño;
            velocidadDeAtaque = armaData.velocidadDeAtaque;
        }
    }

    public override void Atacar()
    {
        // Implementación del ataque de la cuchara
        Debug.Log(nombreArma + " atacando con " + daño + " de daño.");
    }

    public override void Pasiva()
    {
        // Implementación de la habilidad pasiva de la cuchara
        Debug.Log("Habilidad pasiva de " + nombreArma + " activada.");
    }

    public override void Activa()
    {
        // Implementación de la habilidad activa de la cuchara
        Debug.Log("Habilidad activa de " + nombreArma + " activada.");
    }
}
