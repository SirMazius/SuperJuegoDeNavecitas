using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/*
    Cuandos se cree la entidad la asociamos con el empty que la siga. 
*/
[DisallowMultipleComponent]
public class SetEntityToFollow : MonoBehaviour, IConvertGameObjectToEntity
{

    public GameObject follower;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        follower.GetComponent<FollowEntity>().playerEntity = entity;       
    }
}
