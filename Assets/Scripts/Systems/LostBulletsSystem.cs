using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

/*
    Este sistema se encarga de recoger las balas que se pierdan y eliminarlas. 

    NOTA: Podriamos simplicar todo este sistema si cogieramos el (0,0,0) como referencia
    pero bueno ... Asi hemos probado a liberar un NaviteArray desde un job.
*/
public class LostBulletsSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem endSimulationCommandBuffer;

    protected override void OnCreate()
    {
        endSimulationCommandBuffer = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }
    protected override void OnUpdate()
    {
        // Preparamos un buffer para eliminar las balas que se hayan salido de la vision al acabar el frame.
        EntityCommandBuffer.ParallelWriter localBuffer = endSimulationCommandBuffer.CreateCommandBuffer().AsParallelWriter();
        // Definimos una Query que busque a los jugadores y recuperamos sus entidades.
        EntityQuery playersQuery = GetEntityQuery(typeof(PlayerTag));
        int nPlayer = playersQuery.CalculateEntityCount();

        NativeArray<Entity> playersEntities = playersQuery.ToEntityArray(Allocator.TempJob);
        
        // Recuperamos todos los componentes que tienen Translations.
        
        var translationList = GetComponentDataFromEntity<Translation>(true);

        // Para cada bala NOTA: entityInQueryIndex es una palabra reservada nos sirve para que saber el indice de esa entidad dentro de la query, necesario para poder operar en paralelo.
        // NOTA: Los WithReadOnly son super importantes si trabajanmos en paralelo sino el compilador no se fia.
        Entities.WithReadOnly(translationList).WithReadOnly(playersEntities).WithAll<BulletTag>().ForEach((Entity bulletEntity, int entityInQueryIndex/*, ref Translation trans*/) => { // Nota a pie de codigo 1*

            // Comprobamos si nos hemos alejado bastante de los jugadores.
            for (int i = 0; i < nPlayer; i++)
            {
                var player = playersEntities[i];
                // Antes de acceder al transform, tenemos que comprobar si los transform recogidos tienen el de nuestro jugador.
                if (translationList.HasComponent(player)) 
                {

                    if (math.distance(translationList[bulletEntity].Value, translationList[player].Value) > 100.0f)
                    {
                        localBuffer.DestroyEntity(entityInQueryIndex, bulletEntity);
                    }
                }
            }
            // Esto esta preparado para que trabaje en paralelo pero es un overkill en toda regla no van a haber > 100 balas en pantalla ni de conya, igual es hasta mas lento.
        }).WithDisposeOnCompletion(playersEntities).ScheduleParallel();


        endSimulationCommandBuffer.AddJobHandleForProducer(Dependency);
    }
}

/*
 1* si dejamos ese translation (trans) vamos a tener un problema de aliasing en memoria
    porque estaremos accediendo a un array de translations desde la query implicita del ForEach
    y adenas desde la query de GetComponentDataFromEntity que rellena translationList.
*/
