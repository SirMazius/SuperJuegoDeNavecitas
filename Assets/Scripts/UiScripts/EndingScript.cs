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
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        foreach (Entity e in entityManager.GetAllEntities())
        {
            entityManager.DestroyEntity(e);
        }
        scoreText.SetText(@"<material=Liberation2>SCORE: " + gameData.finalScore);
    }

    // Update is called once per frame
    void Update()
    {
        if (PressAnyButton.wasAnyButtonPressed())
        {
            SceneManager.LoadScene(0);
        }
    }
}
