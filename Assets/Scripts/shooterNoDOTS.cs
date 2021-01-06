using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class shooterNoDOTS : MonoBehaviour
{
    
    public GameObject bullet;
    public int counter;
    BlobAssetStore blobA;

    private void Start()
    {
        blobA = new BlobAssetStore();
        var eM = World.DefaultGameObjectInjectionWorld.EntityManager;
        var bulletEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(bullet, GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobA));
        counter = 0;
        int max = 200;
        for (int i = 0; i < max; i++)
        {
            for (int j = 0; j < max; j++)
            {

                var auxEntity = eM.Instantiate(bulletEntity);
                Translation cubeTranslation = new Translation {
                    Value = new float3(i * 0.5f, 0, j * 0.5f)
                };
            eM.SetComponentData(auxEntity, cubeTranslation);
            }
        }
    }

    private void OnDisable()
    {
        blobA.Dispose();
    }
    // Start is called before the first frame update

    // Update is called once per frame

}
