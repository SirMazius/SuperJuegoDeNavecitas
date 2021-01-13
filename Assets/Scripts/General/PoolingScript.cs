using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

/* 
    Clase que nos sirve para instanciar los efectos visuales de forma eficiente.
*/
public class PoolingScript : MonoBehaviour
{
    private Dictionary<string, Queue<GameObject>> effectsDictionary;

    public int nMaxEffects;
    public static PoolingScript instance;
    public GameObject[] particleSystemTypes;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            PopulateEffectsDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowEffectAtPosition(string effectType, Vector3 newPosition)
    {
        if (effectsDictionary.ContainsKey(effectType))
        {
            /* 
               Buscamos el tipo de efecto visual, lo mostramos en la posicion que toque 
               y lo ponemos en la cola otra vez para que pueda volver a ser utilizado.
            */
            GameObject particleEffect = effectsDictionary[effectType].Dequeue();
            ParticleSystem particlesSys = particleEffect.GetComponent<ParticleSystem>();
            particleEffect.transform.position = newPosition;
            if (particlesSys.isPlaying)
            {
                particlesSys.Clear();
            }
            particlesSys.Play();
            effectsDictionary[effectType].Enqueue(particleEffect);
        }
    }

    public void ShowEffectAtMultiplePositions(string effectType, NativeList<float3> positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            ShowEffectAtPosition(effectType, positions[i]);
        }
    }

    private void PopulateEffectsDictionary()
    {
        effectsDictionary = new Dictionary<string, Queue<GameObject>>();

        for (int i = 0; i < particleSystemTypes.Length; i++)
        {
            string keyValue = particleSystemTypes[i].name;
            for (int j = 0; j < nMaxEffects; j++)
            {
                if (effectsDictionary.ContainsKey(keyValue))
                {
                    GameObject particlesGo = Instantiate(particleSystemTypes[i]);
                    particlesGo.GetComponent<ParticleSystem>().Stop();
                    effectsDictionary[keyValue].Enqueue(particlesGo);
                }
                else
                {
                    effectsDictionary.Add(keyValue, new Queue<GameObject>());
                }
            }
        }
    }
}
