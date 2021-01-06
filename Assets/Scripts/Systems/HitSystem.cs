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
        [ReadOnly] public ComponentDataFromEntity<PlayerTag> playersArray; // TODO: Sustituir esto por enemyTag.
        [ReadOnly] public ComponentDataFromEntity<BulletTag> bulletsArray;
        // Aqui encolaremos las operaciones de destruir las entidades.
        public EntityCommandBuffer jobCommandBuffer;

        // Se ejecutara una vez por cada trigger que salte.
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity EntityA = triggerEvent.EntityA;
            Entity EntityB = triggerEvent.EntityB;

            

            if (playersArray.HasComponent(EntityA) && bulletsArray.HasComponent(EntityB))
            {
                
                jobCommandBuffer.DestroyEntity(EntityA);
            }
            else if (playersArray.HasComponent(EntityB) && bulletsArray.HasComponent(EntityA))
            {
                jobCommandBuffer.DestroyEntity(EntityB);
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

        bulletJob.playersArray = GetComponentDataFromEntity<PlayerTag>(true);
        bulletJob.bulletsArray = GetComponentDataFromEntity<BulletTag>(true);
        bulletJob.jobCommandBuffer = instantiateEntitiesCommandBuffer.CreateCommandBuffer();

        JobHandle outputDeps = bulletJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);
        /*
            IMPORTANTE: Esto es necesario, por defecto SystemBase gestiona las dependencias en automantico PERO
            BulletOnHitTriggerJob es un Job especial, el y todos los que no sean del tipo Entities.ForEach... no se gestionan en automatico.
        */
        Dependency = JobHandle.CombineDependencies(outputDeps, Dependency);
        //outputDeps.Complete();
        instantiateEntitiesCommandBuffer.AddJobHandleForProducer(outputDeps);
    }
}

