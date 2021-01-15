using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
/*
    Esta clase gestiona la condicion de derrota del jugador. 
*/
// SJDN -> SuperJuegoDeNavecitas.
namespace SJDN // Como somos bastante listos este sceneManager enmascara al de Unity lo que da problemas con el namespace lo solventamos.
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager instance;
        public TMPro.TextMeshProUGUI scoreText;
        public TMPro.TextMeshProUGUI healthText;
        public GameDataSO gameData;
        public AudioSource audioSource;
        public int healthRemaining;
        private InputSystem inputSystem;
        private bool enableLasserSound;

        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
                gameData.finalScore = 0;
                // Inicializamos los input del jugador ahora que sabemos en que escena nos encontramos.
                inputSystem = World.DefaultGameObjectInjectionWorld.CreateSystem<InputSystem>();
                inputSystem.Enabled = true;
                enableLasserSound = true;
                //inputSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InputSystem>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Forzamos la actualizacion... por algun motivo no lo hace en automatico y eso que se lo especificamos.
            inputSystem.Update();
            // Se acaba la partida.
            if (healthRemaining <= 0)
            {
                /*
                    En lugar de cepillarnos aqui todas las entidades vamos a hacerlo en un nuevo sistema una vez se cargue el nivel. 
                */
                Debug.Log("N ENTIDADES ANTES -> " + World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities().Length);
                // Nos deshacemos del input sistem y cargamos la escena.
                
                Debug.Log("N ENTIDADES ANTES -> " + World.DefaultGameObjectInjectionWorld.EntityManager.GetAllEntities().Length);
                //World.DefaultGameObjectInjectionWorld.DestroySystem(inputSystem);
                //World.DefaultGameObjectInjectionWorld.EntityManager.
                Debug.Log("ALGO");
                World.DefaultGameObjectInjectionWorld.DestroySystem(inputSystem);
                UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
            }
        }

        public void LoseHealth()
        {
            healthRemaining--;
            UpdateHealthUi();
        }

        public void AddHealth()
        {
            healthRemaining++;
            UpdateHealthUi();
        }

        // TODO: mejorar esto para que ponga los +++ en vez de un numero.
        public void UpdateHealthUi()
        {
            healthText.SetText(@"VIDAS <material=Liberation2>" + healthRemaining.ToString());
        }

        public void AddScore(int score)
        {
            gameData.finalScore += score;
            UpdateScoreUi();
        }

        public void UpdateScoreUi()
        {
            scoreText.SetText(@"SCORE <material=Liberation2>" + gameData.finalScore.ToString());
        }

        struct cleanJob : IJob
        {
            public void Execute()
            {
                
            }
        }

        public void PlayLaserSound()
        {
            if (enableLasserSound)
            {
                instance?.audioSource.PlayOneShot(instance.audioSource.clip);
                StartCoroutine("EnableLaserSound");
            }
                
        }

        IEnumerator EnableLaserSound()
        {
            enableLasserSound = false;
            yield return new WaitForSeconds(0.1f);
            enableLasserSound = true;
        }
    }

}
