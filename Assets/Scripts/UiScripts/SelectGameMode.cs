using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectGameMode : MonoBehaviour
{
    public void play1playerGame()
    {
        SceneManager.LoadScene("1Jugador");
    }

    public void play2PlayerGame()
    {
        SceneManager.LoadScene("2Jugador");
    }
}
