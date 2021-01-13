using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
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
        public int healthRemaining;
        private InputSystem inputSystem;
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

                UnityEngine.SceneManagement.SceneManager.LoadScene("EndScene");
                //    Debug.Log("FIN de la partida");
                //    // TODO: Ejecutar una query que marque para destruiar a las naves de los enemigos. y ejecutar una corutina que nos cambie de escena.
                //    /*
                //        EL SISTEMA QUE DESTRUYE A LOS ENEMIGOS ES EL ENCARCAGADO DE INSTANCIA EXPLOSIONES DESDE EL POOLER. 
                //        CREAMOS OTRO SISTEMA QUE SE ENCARGUE DE ESTRUIR A LOS JUGADORES E INTANCIAR TAMBIEN DESDE EL POOLER.
                //    */
                //}
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

        private void OnDisable()
        {
            World.DefaultGameObjectInjectionWorld.DestroySystem(inputSystem);
        }
    }

}
