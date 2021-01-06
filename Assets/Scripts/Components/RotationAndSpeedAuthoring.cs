using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RotationAndSpeedAuthoring : IComponentData
{
    [Tooltip("Velocidad maxima que puede alcanzar esta entidad")]
    public float speed;
    [Tooltip("Rotacion maxima que puede alcanzar esta entidad en grados")]
    public float rotationSpeed; // Rotacion en grados.
}
