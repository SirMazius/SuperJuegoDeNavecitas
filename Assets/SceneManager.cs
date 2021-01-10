using System.Collections;
using System.Collections.Generic;
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
        public int healthRemaining;
        public int currentScore;
        // Start is called before the first frame update
        void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
            // Se acaba la partida.
            //if (healthRemaining <= 0)
            //{
            //    Debug.Log("FIN de la partida");
            //    // TODO: Ejecutar una query que marque para destruiar a las naves de los enemigos. y ejecutar una corutina que nos cambie de escena.
            //    /*
            //        EL SISTEMA QUE DESTRUYE A LOS ENEMIGOS ES EL ENCARCAGADO DE INSTANCIA EXPLOSIONES DESDE EL POOLER. 
            //        CREAMOS OTRO SISTEMA QUE SE ENCARGUE DE ESTRUIR A LOS JUGADORES E INTANCIAR TAMBIEN DESDE EL POOLER.
            //    */
            //}
        }

        public void LoseHealth()
        {
            healthRemaining--;
            //UpdateHealthUi();
        }

        public void AddHealth()
        {
            healthRemaining++;
            UpdateHealthUi();
        }

        // TODO: mejorar esto para que ponga los +++ en vez de un numero.
        public void UpdateHealthUi()
        {
            scoreText.SetText(healthRemaining.ToString());
        }

        public void AddScore(int score)
        {
            currentScore += score;
            UpdateScoreUi();
        }

        public void UpdateScoreUi()
        {
            scoreText.SetText(currentScore.ToString());
        }
    }

}
