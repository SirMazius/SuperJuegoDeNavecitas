﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectGameMode : MonoBehaviour
{
    public GameObject Player1Button;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(Player1Button);
    }

    public void play1playerGame()
    {
        // Eliminamos a los voids que pululan por la escena.
        VoidsSystem.DestroyVoidsInScene(); 
        // Cargamos el modo un jugador.
        SceneManager.LoadScene("1Jugador");
    }

    public void play2PlayerGame()
    {
        // Eliminamos a los voids que deambulan por ahi.
        VoidsSystem.DestroyVoidsInScene();
        // Cargamos la escena.
        SceneManager.LoadScene("2Jugador");
    }
}
