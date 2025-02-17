using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    public GameObject otroObjetoPrefab;
    private bool soltandoObjeto = false; // Variable para evitar múltiples corrutinas

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator SoltarObjetoCadaIntervalo(float intervalo)
    {
        while (persiguiendoJugador)
        {
            yield return new WaitForSeconds(intervalo);
            if (puedeSoltarObjeto)
            {
                Vector3 dropPosition = this.dropPosition != null ? this.dropPosition.position : transform.position;
                RaycastHit hit;
                if (Physics.Raycast(dropPosition, Vector3.down, out hit))
                {
                    dropPosition.y = hit.point.y + 0.1f;
                }

                GameObject objetoInstanciado = Instantiate(otroObjetoPrefab, dropPosition, Quaternion.identity);

                // Iniciar la disminución del albedo si el objeto instanciado tiene el componente Charco
                Charco charco = objetoInstanciado.GetComponent<Charco>();
                if (charco != null)
                {
                    charco.IniciarDisminucion();
                }
            }
        }
        soltandoObjeto = false; // Restablecer la variable cuando la corrutina termine
    }

    protected override void Update()
    {
        base.Update();
        if (persiguiendoJugador && puedeSoltarObjeto && !soltandoObjeto)
        {
            soltandoObjeto = true; // Marcar que la corrutina está en ejecución
            StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
        }
    }
}


