using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class BulletHitSystem : SystemBase
{

    private BuildPhysicsWorld buildPhysicsWorld; // El mundo donde se inicializa la simulacion (genran los colliders, inicializan los rigid bodies...).
    private StepPhysicsWorld stepPhysicsWorld; // El mundo donde se desarrolla el calculo de las simulaciones.

    /*
        Job encargado de manejar las colisiones entre el enemigo y el jugador. 
    */
    struct EnemyOnHitTriggerJob : ITriggerEventsJob
    {

        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemiesArray;
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playersArray;
        [WriteOnly] public ComponentDataFromEntity<PlayerHitted> playerHittedArray;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            /*
                Cuando un jugador choque contra un enemigo, el jugador quedara herido, 
                otro sistema generara una honda expansiva que destruya a los enemigos cercanos
                y reste vida al jugador.
            */
            if (enemiesArray.HasComponent(entityA) && playersArray.HasComponent(entityB)) 
            {
                playerHittedArray[entityB] = new PlayerHitted() { wasPlayerHitted = true};
            }
            else if (enemiesArray.HasComponent(entityB) && playersArray.HasComponent(entityA))
            {
                playerHittedArray[entityA] = new PlayerHitted() { wasPlayerHitted = true };
            }
        }
    }

    /*
         Job encargado de manejar las colisiones entre el enemigo y las balas.
    */
    struct BulletOnHitTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemiesArray;
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletsArray;
        [WriteOnly] public ComponentDataFromEntity<EnemyData> enemyDataArray;

        // Se ejecutara una vez por cada trigger que salte.
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity EntityA = triggerEvent.EntityA;
            Entity EntityB = triggerEvent.EntityB;

            /*
                Tenemos unos flags en las entidades de los enemigos que las marcan como pendientes de destruir
                mediante una query comprobara cuales hay que destruir y sumamos la puntuacion,
                finalmente otro sistema las destruira.
            */
            // En este caso EntityA seria un enemigo y EntityB la bala y viceversa.
            if (enemiesArray.HasComponent(EntityA) && bulletsArray.HasComponent(EntityB))
            {
                enemyDataArray[EntityA] = new EnemyData() { shouldBeDestroyed = true};
            }
            else if (enemiesArray.HasComponent(EntityB) && bulletsArray.HasComponent(EntityA))
            {
                enemyDataArray[EntityB] = new EnemyData() { shouldBeDestroyed = true };
            }
        }
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        // Inicializamos los simulation worlds.
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }


    protected override void OnUpdate()
    {
        var enemiesArray = GetComponentDataFromEntity<EnemyTag>(true);
        var enemyDataArray = GetComponentDataFromEntity<EnemyData>();
        // Creamos el job que maneja la interaccion bala enemigo.
        var bulletJob = new BulletOnHitTriggerJob(); 

        bulletJob.enemiesArray = enemiesArray;
        bulletJob.enemyDataArray = enemyDataArray;
        bulletJob.bulletsArray = GetComponentDataFromEntity<BulletTag>(true);


        JobHandle bulletSysOutputDeps = bulletJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);

        var playersArray = GetComponentDataFromEntity<PlayerTag>();
        var playerHitted = GetComponentDataFromEntity<PlayerHitted>();
        // Creamos el job que maneja la interaccion jugador enemigo.
        var enemyJob = new EnemyOnHitTriggerJob();
        enemyJob.playerHittedArray = playerHitted;
        enemyJob.enemiesArray = enemiesArray;
        enemyJob.playersArray = playersArray;

        JobHandle enemySysOutputDeps = enemyJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);

        #region
        //EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag));
        //var playersNativeArray = playerQuery.ToEntityArray(Allocator.TempJob);
        //int nPlayers = playersNativeArray.Length;
        //var translationArray = GetComponentDataFromEntity<Translation>();
        // TODO: Pasar esto a DestroyEnemies.cs sera mas facil...
        // Este job se encarga de marcar para destruir todos los enemigos cercanos a un jugador dañado
        //JobHandle expansiveWave = Entities.WithAll<EnemyTag>().ForEach((Entity enemyEntity) => {
        //    float3 enemyPosition = translationArray[enemyEntity].Value;
        //    for (int i = 0; i < nPlayers; i++)
        //    {
        //        Entity playerEntity = playersNativeArray[i];
        //        if (playerHitted[playerEntity].wasPlayerHitted 
        //            && math.length(translationArray[enemyEntity].Value - translationArray[playerEntity].Value) < 12) // TODO: establecer una variable en vez del 12.5f.
        //        {
        //            enemyDataArray[enemyEntity] = new EnemyData() { shouldBeDestroyed = true};
        //        }
        //    }
        //}).WithDisposeOnCompletion(playersNativeArray).Schedule(JobHandle.CombineDependencies(bulletSysOutputDeps, enemySysOutputDeps));
        #endregion
        /*
            TODO: (REVISAR COMENTARIO) IMPORTANTE: Esto es necesario, por defecto SystemBase gestiona las dependencias en automantico PERO
            BulletOnHitTriggerJob es un Job especial, el y todos los que no sean del tipo Entities.ForEach... no se gestionan en automatico.
        */

        // Aqui comprobamos cuantos de los enemigos se han destruido por un impacto de bala y lo almacenamos en el array.
        NativeArray<int> destroyedEnemiesCounterArray = new NativeArray<int>(new int[]{0}, Allocator.TempJob);
        var counterDependency = Entities.WithAll<EnemyTag>().ForEach((in EnemyData enemyData) =>
        {
            if (enemyData.shouldBeDestroyed)
            {
                destroyedEnemiesCounterArray[0] += 1;
            }
        }).Schedule(bulletSysOutputDeps);

        // Esperamos a que se termine de contar.
        counterDependency.Complete();
        // Asignamos la nueva puntuacion.
        SJDN.SceneManager.instance.AddScore(destroyedEnemiesCounterArray[0] * 100);
        //TextUpdater.textUpdaterInstance.SetScore(destroyedEnemiesCounterArray[0] * 100);
        // Liberamos el array con la informacion extraida del job que cuenta las naves a destruir.
        destroyedEnemiesCounterArray.Dispose();

        
        Dependency = JobHandle.CombineDependencies(enemySysOutputDeps, Dependency); // TODO: enemySysOutputDeps o expansiveWave?
    }
}

