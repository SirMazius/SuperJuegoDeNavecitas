using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct CharacterControllerInputData : IComponentData
{
    public float2 Movement;
    public float2 Looking;
    public bool shooting;
}
