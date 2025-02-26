using System.Collections;
using UnityEngine;

public class EnemigoCuerpo : Enemigo
{
    public GameObject otroObjetoPrefab;
    private Coroutine soltarObjetoCoroutine; // Variable para almacenar la corrutina
    private bool haAlcanzadoAlJugador = false; // Variable para controlar si ha alcanzado al jugador

    protected override void Start()
    {
        base.Start();
        soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
    }

    private IEnumerator SoltarObjetoCadaIntervalo(float intervalo)
    {
        while (true)
        {
            yield return new WaitForSeconds(intervalo);
            if (persiguiendoJugador && puedeSoltarObjeto)
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
    }

    protected override void Update()
    {
        base.Update();
        // No es necesario detener y reiniciar la corrutina aquí
    }

    public override void PerseguirJugador()
    {
        base.PerseguirJugador();
        if (haAlcanzadoAlJugador)
        {
            if (soltarObjetoCoroutine != null)
            {
                StopCoroutine(soltarObjetoCoroutine);
            }
            soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
            haAlcanzadoAlJugador = false;
        }
    }

    public void AlcanzarJugador()
    {
        haAlcanzadoAlJugador = true;
    }
}
