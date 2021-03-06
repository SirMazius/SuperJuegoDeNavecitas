﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class OptionsScript : MonoBehaviour
{
    public VolumeProfile postProcessingVolumeEpileptic;
    public VolumeProfile postProcessingVolumeNonEpileptic;
    public GameObject postProcessingVolume;
    public GameObject toggle;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(toggle);
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;    
    }

    public void SetEpileptic(bool amIepileptic)
    {
        if (amIepileptic)
        {
            postProcessingVolume.GetComponent<Volume>().profile = postProcessingVolumeEpileptic;
        }
        else
        {
            postProcessingVolume.GetComponent<Volume>().profile = postProcessingVolumeNonEpileptic;
        }
    }

    public void SetGraphicSettings(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }
}
