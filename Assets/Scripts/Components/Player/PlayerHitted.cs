﻿using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
[Serializable]
public struct PlayerHitted : IComponentData
{
    public bool wasPlayerHitted;   
}
