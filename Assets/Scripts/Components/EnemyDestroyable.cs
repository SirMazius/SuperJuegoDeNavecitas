﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct EnemyData : IComponentData
{
    public bool shouldBeDestroyed;
}
