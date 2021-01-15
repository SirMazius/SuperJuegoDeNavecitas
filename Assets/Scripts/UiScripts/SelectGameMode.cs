using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectGameMode : MonoBehaviour
{
    public GameObject Player1ButtonGo;
    public GameObject Player2ButtonGo;
    private  Button player2Button;
    private void Update()
    {
        if (Gamepad.all.Count != 2)
        {
            player2Button.interactable = false;
        }
        else
        {
            player2Button.interactable = true;
        }
    }

    private void OnEnable()
    {
        player2Button = Player2ButtonGo.GetComponent<Button>();
        EventSystem.current.SetSelectedGameObject(Player1ButtonGo);
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
