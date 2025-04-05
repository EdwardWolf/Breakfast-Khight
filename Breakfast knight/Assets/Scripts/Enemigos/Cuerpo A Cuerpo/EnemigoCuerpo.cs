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
        if (enContactoConEscudo)
        {
            velocidadMovimientoActual = 0f;
        }
        else
        {
            base.PerseguirJugador();
            if (haAlcanzadoAlJugador)
            {
                velocidadMovimientoActual = 0f;
                if (soltarObjetoCoroutine != null)
                {
                    StopCoroutine(soltarObjetoCoroutine);
                }
                soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
                haAlcanzadoAlJugador = false;
            }
            else
            {
                velocidadMovimientoActual = velocidadMovimientoInicial;
            }
        }
    }


    public void AlcanzarJugador()
    {
        haAlcanzadoAlJugador = true;
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        if (collision.gameObject.CompareTag("Escudo"))
        {
        //    enContactoConEscudo = true;
            if (soltarObjetoCoroutine != null)
            {
               StopCoroutine(soltarObjetoCoroutine);
           }
        //    if (jugador != null)
        //    {
        //        reducirResistenciaEscudoCoroutine = StartCoroutine(EsperarYReducirResistenciaEscudo());
        //    }
        }
    }

    protected override void OnCollisionExit(Collision collision)
    {
        base.OnCollisionExit(collision);
        if (collision.gameObject.CompareTag("Escudo"))
        {
        //    enContactoConEscudo = false;
        //    if (reducirResistenciaEscudoCoroutine != null)
        //    {
        //        StopCoroutine(reducirResistenciaEscudoCoroutine);
        //        reducirResistenciaEscudoCoroutine = null;
        //    }
            if (soltarObjetoCoroutine == null)
           {
               soltarObjetoCoroutine = StartCoroutine(SoltarObjetoCadaIntervalo(tiempoParaSoltarObjeto));
           }
        }
    }
}
