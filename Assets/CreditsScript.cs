using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreditsScript : MonoBehaviour
{
    public GameObject backButton;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(backButton);
    }

}
