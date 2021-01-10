using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;


public class MoveFordwardSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        
        Entities.WithAll<BulletTag>().ForEach((ref Translation translation, in LocalToWorld localToWorld, in RotationAndSpeedAuthoring rotNspeed) => {

            float2 moveDirection = math.normalizesafe(new float2(localToWorld.Forward.x, localToWorld.Forward.z));
            float3 moveDirection3d = new float3(moveDirection.x, 0.0f, moveDirection.y);
            translation.Value += moveDirection3d * rotNspeed.speed * deltaTime;

        }).ScheduleParallel();
    }
}
