using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ReiniciarEscena : MonoBehaviour , IPointerClickHandler
{
    private GameManager gameManager;
    private void OnEnable()
    {
        gameManager = GameManager.instance;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameManager.ReiniciarJuego();
    }
}

