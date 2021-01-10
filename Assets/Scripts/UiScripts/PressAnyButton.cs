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
        bool keyPressed = Keyboard.current.anyKey.wasPressedThisFrame;
        bool buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame
            || Gamepad.current.buttonWest.wasPressedThisFrame
            || Gamepad.current.buttonNorth.wasPressedThisFrame
            || Gamepad.current.buttonEast.wasPressedThisFrame;

        return keyPressed || buttonPressed;
    }

    private void Start()
    {
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
