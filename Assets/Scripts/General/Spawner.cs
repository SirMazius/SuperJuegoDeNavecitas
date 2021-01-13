using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;
using Unity.Transforms;

public class Spawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject playerFollower1;
    public GameObject playerFollower2;
    EntityManager entityManager;
    Entity enemyEntity;
    BlobAssetStore blobAsset;
    public float wavesInterval;
    public float circleRadious;
    public int initialEnemies;
    float time;
    private bool freshWave;
    // Start is called before the first frame update
    EndSimulationEntityCommandBufferSystem instantiateEntitiesCommandBuffer;

    void Start()
    {
        time = wavesInterval;
        freshWave = false;
        instantiateEntitiesCommandBuffer = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        
        blobAsset = new BlobAssetStore(); 
        GameObjectConversionSettings conversionSettings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAsset);
        enemyEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, conversionSettings);
        //
    }
    
    /*
        A cada oleada instanciamos 20 enemigos. si antes de instanciar la siguiente oleada se han eliminado todos los enemigos
        reducimos el tiempo entre oleadas.
        Los enemigos se instancian en un punto generado aleatoriamente en un circulo al rededor de los jugadores.
    */
    void Update()
    {
        //EntityCommandBuffer commandBuffer = instantiateEntitiesCommandBuffer.CreateCommandBuffer();
        if (time > wavesInterval)
        {

            float3 circleCenter;
            // Esto significa que solo hay un jugador.
            if (playerFollower2 == null)
            {
                circleCenter = playerFollower1.transform.position;
            }
            else
            {
                circleCenter = (playerFollower1.transform.position + playerFollower2.transform.position) / 2.0f;
            }

            // Generamos un punto en un circulo.
            for (int i = 0; i < initialEnemies; i++)
            {
                float randomAngle = UnityEngine.Random.Range(0, 2 * math.PI);
                float3 randomPos = new float3(math.cos(randomAngle) * circleRadious + circleCenter.x, 0,
                math.sin(randomAngle) * circleRadious + circleCenter.z);

                var instantiatedEnemy = entityManager.Instantiate(enemyEntity);
                entityManager.SetComponentData(instantiatedEnemy, new Translation() { Value = randomPos });
            }

            time = 0;
            freshWave = true;
            //for (int i = 0; i < 20; i++)
            //{
            //    var instantiatedEnemy = entityManager.Instantiate(enemyEntity);
            //    //entityManager.SetComponentData(instantiatedEnemy, new Translation() { Value = randomPos });
            //}


            //var instantiatedEnemy = entityManager.Instantiate(enemyEntity);

            //commandBuffer.SetComponent(instantiatedEntity, new Translation() { Value = randomPos });
            //if (instantiatedEnemy != Entity.Null)
            //{
            //    entityManager.SetComponentData(instantiatedEnemy, new Translation() { Value = randomPos });
            //}
            //

        }
        //Debug.Log(entityManager.CreateEntityQuery(typeof(EnemyTag)).CalculateEntityCount());
        // Si el jugador se ha ventilado a todos los enemigos antes del comienzo de la siguiente ronda reducimos el tiempo.
        if (freshWave && time < wavesInterval && entityManager.CreateEntityQuery(typeof(EnemyTag)).CalculateEntityCount() == 1) // == 1 porque hay una entidad "fantasma" que representa a la que se usa para instanciar al resto de los enemigos.
        {
            wavesInterval = math.clamp(wavesInterval - 1, 0, 20);
            initialEnemies =  (int)(initialEnemies * 1.5f);
            // Limitamos poder bajar el tiempo a una vez por ronda.
            freshWave = false;
        }
        time += Time.deltaTime;
    }

    private void OnDisable()
    {
        // Tenemos que destruir todas las entidades.
    }
    private void OnDestroy()
    {
        blobAsset.Dispose();
    }
}
