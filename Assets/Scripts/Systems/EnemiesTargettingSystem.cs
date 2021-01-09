using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

/*
    Sistema encargado de hacer que los enemigos sigan al jugador tratando de destruirlo. 
*/

public class EnemiesTargettingSystem : SystemBase
{
    [BurstDiscard]
    protected override void OnUpdate()
    {
        // Recogemos las entidades de los jugadores.
        NativeArray<Entity> playerEntitiesArray = GetEntityQuery(typeof(PlayerTag)).ToEntityArray(Allocator.TempJob);
        // Buscamos todos los translation de la escena.
        var translationsArray = GetComponentDataFromEntity<Translation>(true);
        var dt = Time.DeltaTime;
        // Para cada uno de los enemigos buscaremos al jugador mas cercano y nos lanzaremos contra el.
        Entities.WithReadOnly(translationsArray).WithReadOnly(playerEntitiesArray).WithAll<EnemyTag>().ForEach((Entity enemyEntity, ref Rotation rotation, 
            ref PhysicsVelocity enemyVelocity, in EnemyMovilityData movilityData) => {

            // Inicializamos la posicion al infinito para que cualquiera de los dos jugadores este mas cerca.
            float3 targetPosition = new float3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            float3 enemyPosition = translationsArray[enemyEntity].Value;
            Entity targetEntity = Entity.Null;

            foreach(Entity e in playerEntitiesArray)
            {
                if (translationsArray.HasComponent(e))
                {
                    float3 playerPosition = translationsArray[e].Value;
                    // Si la distancia a uno de los jugadores es menor que la de nuestro objetivo actual vamos a por el.
                    if (math.distance(playerPosition, enemyPosition) < math.distance(enemyPosition, targetPosition))
                    {
                        targetPosition = playerPosition;
                        targetEntity = e;
                    }
                }
            }

                // Esto significa que nuestro enemigo ha encontrado un objetivo.
            if (targetEntity != Entity.Null)
            {
                    float3 desired = math.normalize(targetPosition - enemyPosition) * movilityData.maxVelocity;
                    float3 steer = desired - enemyVelocity.Linear;
                    if (math.length(steer) > movilityData.maxRotation)
                    {
                        steer = math.normalize(steer) * movilityData.maxRotation;
                    }
                    enemyVelocity.Linear += steer/* * (1.0f / dt)*/;
                    rotation.Value = quaternion.LookRotationSafe(enemyVelocity.Linear, math.up());
            }

            }).WithDisposeOnCompletion(playerEntitiesArray).Schedule();
    }
}
