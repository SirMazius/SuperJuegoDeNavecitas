using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
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

        // hay que destruir a los jugadores para la siguiente partida.
        CleanPlayerEntities();

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

    /*
        Buscamos todas las entidades del tipo jugador y las destruimos. 
    */
    private void CleanPlayerEntities()
    {
        var eM = World.DefaultGameObjectInjectionWorld.EntityManager;
        NativeArray<Entity> playersEntitiesNarray = eM.CreateEntityQuery(typeof(PlayerTag)).ToEntityArray(Allocator.Temp);

        foreach (Entity playerE in playersEntitiesNarray)
        {
            eM.DestroyEntity(playerE);
        }

        playersEntitiesNarray.Dispose();
    }
}
