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
        /*
            SEEK
        */
        #region
        // Para cada uno de los enemigos buscaremos al jugador mas cercano y nos lanzaremos contra el.
        Entities.WithReadOnly(translationsArray).WithReadOnly(playerEntitiesArray).WithAll<EnemyTag>().ForEach((Entity enemyEntity, ref Rotation rotation, 
            ref PhysicsVelocity enemyVelocity, in EnemyMovilityData movilityData) => { 
                // TODO: Enemy movilityData podria ser un SharedComponen y ahorrariamos algo de ancho de banda.
                
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
                    /*
                        Orientamos al enemigo hacia el jugador limitando sus capacidades de maniobra. 
                    */
                    float3 desired = math.normalize(targetPosition - enemyPosition) * movilityData.maxVelocity;
                    float3 steer = desired - enemyVelocity.Linear;
                    if (math.length(steer) > movilityData.maxRotation)
                    {
                        steer = math.normalize(steer) * movilityData.maxRotation;
                    }
                    enemyVelocity.Linear += steer/* * (1.0f / dt)*/;
                    rotation.Value = quaternion.LookRotationSafe(enemyVelocity.Linear, math.up());
            }

            }).WithDisposeOnCompletion(playerEntitiesArray).Run(); // TODO: Schedule o Parallel.
        #endregion

        
        var enemyQuery = GetEntityQuery(typeof(EnemyTag));
        int nEnemies = enemyQuery.CalculateEntityCount();
        NativeArray<Entity> enemyEntitiesArray = enemyQuery.ToEntityArray(Allocator.TempJob);

        /*
            Esto se encargará de que los enemigos esten algo separados entre si. 
            NOTA: Podriamos emplear alguna tecnica de particionado espacial para mejorar el funcionamiento.
        */
        Entities.WithAll<EnemyTag>().WithReadOnly(translationsArray).ForEach((Entity enemyEntity, ref Rotation rotation, ref PhysicsVelocity enemyVelocity, in EnemyMovilityData emd) => {

            float3 currentEnemyPosition = translationsArray[enemyEntity].Value;
            float3 separationForceSum = new float3();
            int nearEnemies = 0;

            for (int i = 0; i < nEnemies; i++)
            {
                float3 auxEnemyPosition = translationsArray[enemyEntitiesArray[i]].Value;
                // Si otro enemigo esta cerca y no somos ese enemigo.
                float3 vDirector = currentEnemyPosition - auxEnemyPosition;
                float distance = math.length(vDirector);
                if (distance < 5f && enemyEntitiesArray[i] != enemyEntity) // TODO: Ese 1.2f esta muy feo hay que asignarlo en una variable.
                {
                    vDirector = math.normalize(vDirector);
                    vDirector /= distance; // Esto nos permite aplicar una fuerza proporcional en funcion de la cercania del agente.
                    separationForceSum += vDirector;
                    nearEnemies++;
                }
            }

            if (nearEnemies > 0)
            {
                separationForceSum /= nearEnemies;
                separationForceSum = math.normalize(separationForceSum);
                separationForceSum *= emd.maxVelocity;
                float3 steer = separationForceSum - enemyVelocity.Linear;
                if (math.length(steer) > emd.maxRotation)
                {
                    steer = math.normalize(steer) * emd.maxRotation; // 
                }
                enemyVelocity.Linear += steer * 10;/* * (1.0f / dt)*/;
                rotation.Value = quaternion.LookRotationSafe(enemyVelocity.Linear, math.up());
            }

        }).WithDisposeOnCompletion(enemyEntitiesArray).Schedule(); // TODO:  Parallel.
    }
}
