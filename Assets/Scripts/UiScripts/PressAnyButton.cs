using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class PressAnyButton : MonoBehaviour
{
    public GameObject mainMenu;

    public static bool wasAnyButtonPressed()
    {
        // Por lo visto no hay una forma mejor de comprobar cualquier input.
        // bool keyPressed = Keyboard.current.anyKey.wasPressedThisFrame;
        //bool buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame
        //    || Gamepad.current.buttonWest.wasPressedThisFrame
        //    || Gamepad.current.buttonNorth.wasPressedThisFrame
        //    || Gamepad.current.buttonEast.wasPressedThisFrame;

        bool keyPressed = Keyboard.current != null ? Keyboard.current.anyKey.wasPressedThisFrame : false;

        bool buttonPressed;
        if (Gamepad.current != null)
        {
            buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame
            || Gamepad.current.buttonWest.wasPressedThisFrame
            || Gamepad.current.buttonNorth.wasPressedThisFrame
            || Gamepad.current.buttonEast.wasPressedThisFrame;
        }
        else
        {
            buttonPressed = false;
        }

        return keyPressed || buttonPressed;
    }

    /*
        Forzamos FullHD como resolucion si es posible... 
    */
    private void Start()
    {
        bool isFullHDposible = false;
        //Resolution[] resolutionsArray = 
        foreach (Resolution r in Screen.resolutions)
        {
            if (r.width == 1920 && r.height == 1080)
            {
                isFullHDposible = true;
                break;
            }
        }

        if (isFullHDposible)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        mainMenu.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if (wasAnyButtonPressed())
        {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
