using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/*
    Comprueba que naves enemigas han recibido daños y las destruye. 
*/
[UpdateAfter(typeof(BulletHitSystem))]
public class DestroyEnemies : SystemBase
{
    BeginSimulationEntityCommandBufferSystem DestroyEnemyEntitiesCommandBuffer;
    protected override void OnCreate()
    {
        DestroyEnemyEntitiesCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }
    // Buffer que ejecutara la destruccion de las entidades.
    
    protected override void OnUpdate()
    {
        EntityCommandBuffer jobCommandBuffer = DestroyEnemyEntitiesCommandBuffer.CreateCommandBuffer();

        
        NativeArray<Entity> playersEntityNativeArray = GetEntityQuery(typeof(PlayerTag)).ToEntityArray(Allocator.TempJob);
        var translationsArray = GetComponentDataFromEntity<Translation>(true);
        var playerHittedArray = GetComponentDataFromEntity<PlayerHitted>();
        /*
            Iteramos sobre todos los enemigos si han sufrido daños o estan cerca 
            de un jugador que ha sufrido daños son destruidos.
        */
        Entities.WithReadOnly(translationsArray).WithReadOnly(playersEntityNativeArray).WithAll<EnemyTag>().ForEach((Entity enemyEntity, in EnemyData enemyData) => {
            bool isNear = false;
            foreach (var playerE in playersEntityNativeArray)
            {
                // TODO: Quitar el 12.5f hardcodeado.
                if (playerHittedArray[playerE].wasPlayerHitted 
                && math.length(translationsArray[playerE].Value - translationsArray[enemyEntity].Value) < 12.5f)
                {
                    isNear = true;
                }
            }
            if (enemyData.shouldBeDestroyed || isNear)
            {
                jobCommandBuffer.DestroyEntity(enemyEntity);
            }
        }).Schedule();

        // Nos aseguramos que el job que destruye los enemigos ha terminado.
        Dependency.Complete();

        // Solo queremos que puede sufrir daños uno de los dos jugadores a la vez.
        bool allreadyHitted = false;
        foreach (Entity playerE in playersEntityNativeArray)
        {
            if (playerHittedArray[playerE].wasPlayerHitted)
            {
                // Uno de los dos jugadores ha recibido danyos por lo que al otro no le afectaria.
                PoolingScript.instance?.ShowEffectAtPosition("PlayerShockWave", translationsArray[playerE].Value);
                if (!allreadyHitted)
                {
                    allreadyHitted = true;
                    SJDN.SceneManager.instance?.LoseHealth();
                }
            }
            // Los marcamos como no danyados.
            playerHittedArray[playerE] = new PlayerHitted() { wasPlayerHitted = false };
        }

        playersEntityNativeArray.Dispose();

        // Nos aseguramos de que el job haya terminado antes de ejecutar el buffer NOTA: como tenemos un Dependecy.Complete() se
        // sobreentiende que ya habria terminado pero por seguridad mejor lo dejamos.
        DestroyEnemyEntitiesCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
