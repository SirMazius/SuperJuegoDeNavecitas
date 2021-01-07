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
    BeginSimulationEntityCommandBufferSystem instantiateEntitiesCommandBuffer;
    struct BulletOnHitTriggerJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<EnemyTag> enemiesArray;
        [WriteOnly] public ComponentDataFromEntity<EnemyData> enemyDataArray;
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletsArray;
        public EntityCommandBuffer jobCommandBuffer;

        // Se ejecutara una vez por cada trigger que salte.
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity EntityA = triggerEvent.EntityA;
            Entity EntityB = triggerEvent.EntityB;

            /*
                TODO: vamos a tener unos flags en las entidades de los enemigos que las marquen como pendientes de destruir
                mediante una query comprobamos cuales hay que destruir y sumamos la puntuacion,
                finalmente otro sistema las destruira.
            */
            // En este caso EntityA seria un jugador y EntityB la bala y viceversa.
            if (enemiesArray.HasComponent(EntityA) && bulletsArray.HasComponent(EntityB))
            {
                // TextUpdater.textUpdaterInstance.SetScore(100);
                jobCommandBuffer.DestroyEntity(EntityA);
                enemyDataArray[EntityA] = new EnemyData() { shouldBeDestroyed = true};
            }
            else if (enemiesArray.HasComponent(EntityB) && bulletsArray.HasComponent(EntityA))
            {
                // TextUpdater.textUpdaterInstance.SetScore(100);
                jobCommandBuffer.DestroyEntity(EntityB);
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
        // Referenciamos el buffer para ejecutar en el los destroys.
        instantiateEntitiesCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
    }


    protected override void OnUpdate()
    {
        // Creamos un job.
        var bulletJob = new BulletOnHitTriggerJob(); 

        bulletJob.enemiesArray = GetComponentDataFromEntity<EnemyTag>(true);
        bulletJob.enemyDataArray = GetComponentDataFromEntity<EnemyData>();
        bulletJob.bulletsArray = GetComponentDataFromEntity<BulletTag>(true);
        bulletJob.jobCommandBuffer = instantiateEntitiesCommandBuffer.CreateCommandBuffer();


        JobHandle outputDeps = bulletJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);
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
        }).Schedule(outputDeps);

        // Esperamos a que se termine de contar.
        counterDependency.Complete();
        // Asignamos la nueva puntuacion.
        TextUpdater.textUpdaterInstance.SetScore(destroyedEnemiesCounterArray[0] * 100);
        // Liberamos el array con la informacion extraida del job que cuenta las naves a destruir.
        destroyedEnemiesCounterArray.Dispose();

        
        

        // Dependency = JobHandle.CombineDependencies(outputDeps, Dependency);
        instantiateEntitiesCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

