using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

/*
    Este sistema se encarga de que los voids se comporten como floks
    los detalles relacionados con los floks se pueden encontrar aqui:
    https://natureofcode.com/book/chapter-6-autonomous-agents/
*/
public class VoidsSystem : SystemBase
{
    protected override void OnUpdate()
    {

        ComponentDataFromEntity<Translation> translationsArray = GetComponentDataFromEntity<Translation>();

        var voidsQuery = GetEntityQuery(typeof(VoidTag));
        int totalVoids = voidsQuery.CalculateEntityCount();
        var voidsEntitiesNarray = voidsQuery.ToEntityArray(Allocator.TempJob);
        // Comportamiento de separacion.
        // TODO: Codigo duplicado en EnemiesTargettingSystem.
        Entities.WithAll<VoidTag>().ForEach((Entity voidEntity, ref PhysicsVelocity voidVelocity, in EnemyMovilityData movilityData) =>
        {

            float3 currentVoidPosition = translationsArray[voidEntity].Value;
            float3 separationForceSum = new float3();
            float3 currentVelocity = voidVelocity.Linear;
            int nearVoids = 0;

            for (int i = 0; i < totalVoids; i++)
            {
                float3 auxVoidPosition = translationsArray[voidsEntitiesNarray[i]].Value;
                float3 vDirector = currentVoidPosition - auxVoidPosition;
                float distance = math.length(vDirector);
                if (distance < 5.0f && voidEntity != voidsEntitiesNarray[i])
                {
                    vDirector = math.normalize(vDirector);
                    vDirector /= distance;
                    separationForceSum += vDirector;
                    nearVoids++;
                }
            }

            if (nearVoids > 0)
            {
                separationForceSum /= nearVoids;
                separationForceSum = math.normalize(separationForceSum);
                separationForceSum *= movilityData.maxRotation;
                float3 steer = separationForceSum - voidVelocity.Linear;
                if (math.length(steer) > movilityData.maxRotation)
                {
                    steer = math.normalize(steer) * movilityData.maxRotation;
                }
                voidVelocity.Linear = math.normalize(currentVelocity) * movilityData.maxVelocity  + steer * 1.3f; // TODO: Ajustar estos multiplicadores.
                // rotation.Value = quaternion.LookRotationSafe(voidVelocity.Linear, math.up());
            }

        }).Schedule();

        var velocityArray = GetComponentDataFromEntity<PhysicsVelocity>();
        // Comportamiento de alineacion.
        Entities.WithAll<VoidTag>().ForEach((Entity voidEntity, ref Rotation rotation, in EnemyMovilityData movilityData) =>
        {
            float3 alignVelocitySum = new float3();
            float3 currentVelocity = velocityArray[voidEntity].Linear;
            int nearVoids = 0;
            // Iteramos sobre todos los voids.
            for (int i = 0; i < totalVoids; i++)
            {
                float distance = math.length(translationsArray[voidEntity].Value - translationsArray[voidsEntitiesNarray[i]].Value);
                if (distance < 5f && distance > 0.0f)
                {
                    alignVelocitySum += velocityArray[voidsEntitiesNarray[i]].Linear;
                    nearVoids++;
                }

            }

            if (nearVoids > 0)
            {
                alignVelocitySum /= nearVoids;
                alignVelocitySum = math.normalize(alignVelocitySum) * movilityData.maxVelocity;

                float3 steer = alignVelocitySum - currentVelocity;

                if (math.length(steer) > movilityData.maxRotation)
                {
                    steer = math.normalize(steer) * movilityData.maxRotation;
                }

                velocityArray[voidEntity] = new PhysicsVelocity() { Linear = math.normalize(currentVelocity) * movilityData.maxVelocity + steer * 0.3f};
            }

            rotation.Value = quaternion.LookRotationSafe(velocityArray[voidEntity].Linear, math.up());

        }).WithDisposeOnCompletion(voidsEntitiesNarray).Schedule();

        // Comportamiento de cohesion No lo he visto necesario.
        //Entities.WithAll<VoidTag>().ForEach((Entity voidEntity, in EnemyMovilityData movilityData) =>
        //{
        //    float3 alignCohesionPosSum = new float3();
        //    float3 currentVelocity = velocityArray[voidEntity].Linear;
        //    int nearVoids = 0;
        //    // Iteramos sobre todos los voids.
        //    for (int i = 0; i < totalVoids; i++)
        //    {
        //        float distance = math.length(translationsArray[voidEntity].Value - translationsArray[voidsEntitiesNarray[i]].Value);
        //        if (distance < 5f && distance > 0.0f)
        //        {
        //            alignCohesionPosSum += translationsArray[voidsEntitiesNarray[i]].Value;
        //            nearVoids++;
        //        }

        //    }

        //    if (nearVoids > 0)
        //    {
        //        alignCohesionPosSum /= nearVoids;
        //        float3 desiredPos = alignCohesionPosSum - translationsArray[voidEntity].Value;
        //        desiredPos = math.normalize(desiredPos) * movilityData.maxVelocity;
        //        float3 steer = desiredPos - currentVelocity;

        //        velocityArray[voidEntity] = new PhysicsVelocity() { Linear = currentVelocity + steer };
        //    }
        //}).WithDisposeOnCompletion(voidsEntitiesNarray).Schedule();

    }

    /*
        Se encarga de limpiar los voids de la escena. 
    */
    public static void DestroyVoidsInScene()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery voidsQuery = entityManager.CreateEntityQuery(typeof(VoidTag));
        NativeArray<Entity> voidsEntityArray = voidsQuery.ToEntityArray(Allocator.Temp);
        
        foreach (Entity voidShip in voidsEntityArray)
        {
            entityManager.DestroyEntity(voidShip);
        }

        voidsEntityArray.Dispose();
    }
}
