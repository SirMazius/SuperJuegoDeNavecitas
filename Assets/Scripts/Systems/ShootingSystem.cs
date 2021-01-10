using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


/*
    Sistema encargado de instanciar las balas. 
*/

[UpdateInGroup(typeof(SimulationSystemGroup))] // Es el grupo por defecto para los sistemas no seria necesario definirlo.
[UpdateAfter(typeof(InputSystem))]
[UpdateBefore(typeof(BulletHitSystem))]

public class ShootingSystem : SystemBase
{
    // Esto es una referencia al commandBuffer.
    BeginSimulationEntityCommandBufferSystem instantiateEntitiesCommandBuffer;
    //NativeArray<float> time;

    protected override void OnCreate()
    {
        // Inicializamos la referencia.
        instantiateEntitiesCommandBuffer = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        //time = new NativeArray<float>(new float[] {0.0f}, Allocator.Persistent);
        
    }

    protected override void OnUpdate()
    {
        var commandBuffer = instantiateEntitiesCommandBuffer.CreateCommandBuffer();
        var characterInputDataArray = GetComponentDataFromEntity<CharacterControllerInputData>();
        // TODO: Controlar la cadencia.

        Entities.WithAll<CannonData>().ForEach((in Entity entity, in LocalToWorld worldPos, in CannonData cannonData) => {


            // Comprobamos si el jugador se encuentra disparando.
            if (cannonData.player != null && characterInputDataArray.HasComponent(cannonData.player)) // Si el objeto padre existe.
            {
                CharacterControllerInputData playerData = characterInputDataArray[cannonData.player];//EntityManager.GetComponentData<CharacterControllerInputData>(player);

                //// Si esta disparando.
                if (playerData.shooting /*&& time[0] > 0.1f*/ ) // TODOOOOO Necesitamos 2 tiempos uno para cada jugador.
                {
                    // Instanciamos una bala.
                    Entity instantiatedEntity = commandBuffer.Instantiate(cannonData.ammo);
                    /*
                        Solo se puede instanciar en el hilo principal, como estamos en un job que suelen ir a su bola,
                        guardamos las operaciones de instanciar en el command buffer que se ejecutara cuando le toque
                        generando todas las instancias a la vez.
                    */
                    commandBuffer.SetComponent(instantiatedEntity, new Translation { Value = worldPos.Position });
                    commandBuffer.SetComponent(instantiatedEntity, new Rotation { Value = worldPos.Rotation });
                }
            }
            else // Si el objeto padre no existe significa que lo han destruido y hemos de destruirnos nosotros tambien.
            {
                commandBuffer.DestroyEntity(entity);
            }
            
        }).Schedule();

        // El job se ha de haber terminado para cuando se ejecute este buffer.
        instantiateEntitiesCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}
