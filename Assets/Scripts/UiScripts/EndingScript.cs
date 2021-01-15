using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingScript : MonoBehaviour
{
    // Informacion de la puntuacion y otras utilidades.
    public GameDataSO gameData;
    public TMPro.TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        //World.DefaultGameObjectInjectionWorld = new World("Default World");
        //World.DefaultGameObjectInjectionWorld.QuitUpdate = true;
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        scoreText.SetText(@"<material=Liberation2>SCORE: " + gameData.finalScore);
    }

    // Update is called once per frame
    void Update()
    {
        if (PressAnyButton.wasAnyButtonPressed())
        {
            VoidsSystem.DestroyVoidsInScene();
            SceneManager.LoadScene(0);
        }
    }
}
