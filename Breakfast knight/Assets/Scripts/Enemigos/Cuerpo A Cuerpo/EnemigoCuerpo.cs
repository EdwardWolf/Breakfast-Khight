using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    public GameObject otroObjetoPrefab;
    private Coroutine soltarObjetoCoroutine; // Variable para almacenar la corrutina

    protected override void Start()
    {
        base.Start();
    }

    private IEnumerator SoltarObjetoCadaIntervalo(float intervalo)
    {
        while (persiguiendoJugador)
        {
            if(!isDamaging)
            {
                yield return new WaitForSeconds(intervalo);
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
    }

    protected override void Update()
    {
        base.Update();
        if (persiguiendoJugador && soltarObjetoCoroutine == null)
        {
            soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
        }
        else if (!persiguiendoJugador && soltarObjetoCoroutine != null)
        {
            StopCoroutine(soltarObjetoCoroutine);
            soltarObjetoCoroutine = null;
        }
    }
}
