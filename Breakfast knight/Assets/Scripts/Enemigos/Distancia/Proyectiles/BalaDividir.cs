using System.Collections;
using UnityEngine;

public class BalaDividir : Bala
{
    [Header("División de bala")]
    [SerializeField] private float tiempoParaDividir = 1.5f;
    [SerializeField] private float escalaBalaPequena = 0.5f;
    [SerializeField] private int cantidadBalasDivididas = 4;
    [SerializeField] private float velocidadBalasDivididas = 10f;
    public GameObject balaPrefab; // Prefab de la bala hija, asignar en el inspector

    [Header("Variación de división")]
    [SerializeField] private bool usarVariacion = false; // Si es true, usa el nuevo efecto

    private bool yaDividida = false;
    private Coroutine dividirCoroutine;

    protected override void OnEnable()
    {
        base.OnEnable();
        yaDividida = false;
        if (dividirCoroutine != null)
            StopCoroutine(dividirCoroutine);
        dividirCoroutine = StartCoroutine(DividirTrasTiempo());
    }

    protected override void Start()
    {
        base.Start();
        yaDividida = false;
        if (dividirCoroutine != null)
            StopCoroutine(dividirCoroutine);
        dividirCoroutine = StartCoroutine(DividirTrasTiempo());
    }

    private IEnumerator DividirTrasTiempo()
    {
        yield return new WaitForSeconds(tiempoParaDividir);
        if (!yaDividida)
        {
            DividirBala();
        }
    }

    private void DividirBala()
    {
        yaDividida = true;

        if (!usarVariacion)
        {
            // Efecto original: 4 balas en círculo
            float anguloInicial = 0f;
            float incrementoAngulo = 360f / cantidadBalasDivididas;

            for (int i = 0; i < cantidadBalasDivididas; i++)
            {
                float angulo = anguloInicial + i * incrementoAngulo;
                Vector3 direccion = Quaternion.Euler(0, angulo, 0) * transform.forward;
                CrearBalaHija(direccion);
            }
        }
        else
        {
            // Nuevo efecto: 3 balas hacia adelante con 30° de separación
            int cantidad = 3;
            float separacion = 30f; // grados
            float anguloCentral = 0f; // hacia adelante

            for (int i = 0; i < cantidad; i++)
            {
                float angulo = anguloCentral + (i - 1) * separacion;
                Vector3 direccion = Quaternion.Euler(0, angulo, 0) * transform.forward;
                CrearBalaHija(direccion);
            }
        }

        if (attackHandler != null && attackHandler.ataqueActual != null)
        {
            attackHandler.ataqueActual.RegresarBala(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void CrearBalaHija(Vector3 direccion)
    {
        GameObject nuevaBala = Instantiate(balaPrefab, transform.position, Quaternion.LookRotation(direccion));
        nuevaBala.transform.localScale = transform.localScale * escalaBalaPequena;

        Bala balaScript = nuevaBala.GetComponent<Bala>();
        if (balaScript != null)
        {
            balaScript.damage = this.damage * 0.5f;
            balaScript.shieldDamage = this.shieldDamage * 0.5f;
            balaScript.attackHandler = this.attackHandler;
            if (balaScript is BalaDividir dividirScript)
            {
                dividirScript.balaPrefab = this.balaPrefab;
                dividirScript.usarVariacion = this.usarVariacion;
            }
        }

        Rigidbody rb = nuevaBala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direccion.normalized * velocidadBalasDivididas;
        }
    }
}
