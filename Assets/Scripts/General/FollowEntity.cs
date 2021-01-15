using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


/*
    Hace que un empty siga a la entidad del jugador elegido, esto sirve
    para que cinemachine pueda seguir a las entidades.
*/
public class FollowEntity : MonoBehaviour
{

    public Entity playerEntity;

    private EntityManager eM;
    private void Start()
    {
        eM = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    // Empleamos esto para asegurarnos de que los systems se han ejecutado.
    private void LateUpdate()
    {
        if (playerEntity != Entity.Null && eM.Exists(playerEntity))
        {
            float3 playerEntityPosition = eM.GetComponentData<Translation>(playerEntity).Value;
            transform.position = new Vector3(playerEntityPosition.x, playerEntityPosition.y, playerEntityPosition.z);
        }    
    }

}
