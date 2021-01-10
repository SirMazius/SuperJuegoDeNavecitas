using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

// [UpdateAfter(typeof(InputSystem))]
public class PlayerMovementSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float deltaTime = Time.DeltaTime;

        // Iteramos sobre todas las entidades del tipo player, asi podemos mover a los dos jugadores.
        Entities.WithAll<PlayerTag>().ForEach((ref Translation pos, ref Rotation rot, in CharacterControllerInputData cD) =>
        {
            // UnityEngine.Debug.Log(string.Format("X: {0} Y: {1}", cD.Looking.x, cD.Looking.y));
            // Computamos su posicion en base a los datos recogidos por el mandoDeXbox y guardados en CharacterControllerInputData por el sistema InputSystem.
            float2 moveDirection = math.normalizesafe(cD.Movement);
            float3 moveDirection3d = new float3(moveDirection.x, 0.0f, moveDirection.y);
            pos.Value += moveDirection3d * 20 * deltaTime;

            //if (cD.Shooting)
            //{
            //    UnityEngine.Debug.Log("DIIIISPARO");
            //}

            // Calculamos su orientacion en caso de que haya rotacion.
            if (!cD.Looking.Equals(float2.zero))
            {
                float3 Looking3d = new float3(cD.Looking.x, 0.0f, cD.Looking.y);
                Looking3d = math.normalizesafe(Looking3d);
                quaternion targetRotation = quaternion.LookRotationSafe(Looking3d, math.up());
                rot.Value = math.slerp(rot.Value, targetRotation, 0.8f);
            }
        }).Schedule();
        
    }
}
