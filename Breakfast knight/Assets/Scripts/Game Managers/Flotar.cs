using UnityEngine;

public class Flotar : MonoBehaviour
{
    [SerializeField] private float distancia = 1.0f; // Distancia máxima de movimiento
    [SerializeField] private float velocidad = 1.0f; // Velocidad del movimiento
    [SerializeField] private float delay = 0.0f; // Delay para desfasar el movimiento

    private Vector3 posicionInicial;

    // Start se llama antes del primer frame
    void Start()
    {
        // Guardamos la posición inicial del objeto
        posicionInicial = transform.position;
    }

    // Update se llama una vez por frame
    void Update()
    {
        // Calculamos el nuevo desplazamiento en el eje Y usando una función seno con un desfase
        float desplazamiento = Mathf.Sin((Time.time + delay) * velocidad) * distancia;

        // Actualizamos la posición del objeto
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + desplazamiento, posicionInicial.z);
    }
}
