using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EnemyMovilityData : IComponentData
{
    public float maxVelocity;
    public float maxRotation;
}
