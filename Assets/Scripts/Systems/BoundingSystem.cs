using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

public class BoundingSystem : SystemBase
{
    /*
        Cuando una nave llega al margen de la pantalla aparece por el otro. 
    */
    struct VoidOnBorderJob : ITriggerEventsJob
    {
        public ComponentDataFromEntity<Translation> trasnlationArray;
        [ReadOnly] public ComponentDataFromEntity<VoidTag> voidsTagArray;
        [ReadOnly] public ComponentDataFromEntity<BorderData> borderDataArray;

        public EntityCommandBuffer buf;
        

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (voidsTagArray.HasComponent(entityA) && borderDataArray.HasComponent(entityB))
            {
                //buf.DestroyEntity(entityA);
                SetNewPosition(entityA, entityB);
            }
            else if (voidsTagArray.HasComponent(entityB) && borderDataArray.HasComponent(entityA))
            {
                //buf.DestroyEntity(entityB);
                SetNewPosition(entityB, entityA);
            }

        }

        /*
            En funcion de la pared con la que choquemos movemos al void a la pared contraria.
        */
        public void SetNewPosition(Entity voidEnemy, Entity border)
        {
            float3 newPosition = new float3();
            float3 currentPosition = trasnlationArray[voidEnemy].Value;
            // Comprobamos contra que pared ha chocado.
            switch (borderDataArray[border].region)
            {
                case Constants.NORTH:
                    newPosition = new float3(currentPosition.x, currentPosition.y, -45.0f);
                    break;
                case Constants.EAST:
                    newPosition = new float3(4.0f, currentPosition.y, currentPosition.z);
                    break;
                case Constants.SOUTH:
                    newPosition = new float3(currentPosition.x, currentPosition.y, 1.0f);
                    break;
                case Constants.WEST:
                    newPosition = new float3(72.0f, currentPosition.y, currentPosition.z);
                    break;
            }
            // Asignamos la nueva posicion.
            trasnlationArray[voidEnemy] = new Translation() { Value = newPosition };

        }

    }

    BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld stepPhysicsWorld;
    EndSimulationEntityCommandBufferSystem DestroyEnemyEntitiesCommandBuffer;
    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        DestroyEnemyEntitiesCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        VoidOnBorderJob voidsBorderJob = new VoidOnBorderJob();
        voidsBorderJob.trasnlationArray = GetComponentDataFromEntity<Translation>();
        voidsBorderJob.voidsTagArray = GetComponentDataFromEntity<VoidTag>(true);
        voidsBorderJob.borderDataArray = GetComponentDataFromEntity<BorderData>(true);
        voidsBorderJob.buf = DestroyEnemyEntitiesCommandBuffer.CreateCommandBuffer();

        JobHandle borderDependecy = voidsBorderJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, this.Dependency);
        DestroyEnemyEntitiesCommandBuffer.AddJobHandleForProducer(borderDependecy);
        Dependency = JobHandle.CombineDependencies(Dependency, borderDependecy);

    }
}
