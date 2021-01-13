using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitScript : MonoBehaviour
{

    public void Play()
    {
        SceneManager.LoadScene("2Jugador");
    }
    // Start is called before the first frame update
    public void Quit()
    {
        Application.Quit();
    }
}
