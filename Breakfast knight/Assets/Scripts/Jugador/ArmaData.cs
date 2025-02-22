using UnityEngine;

[CreateAssetMenu(fileName = "NuevaArma", menuName = "Arma")]
public class ArmaData : ScriptableObject
{
    public string nombreArma;
    public float daño;
    public float velocidadDeAtaque;
}
