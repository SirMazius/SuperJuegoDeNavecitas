using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PressAnyButton : MonoBehaviour
{
    public GameObject mainMenu;

    private void Start()
    {
        mainMenu.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        // Por lo visto no hay una forma mejor de comprobar cualquier input.

        bool keyPressed = Keyboard.current.anyKey.wasPressedThisFrame;
        bool buttonPressed = Gamepad.current.buttonSouth.wasPressedThisFrame
            || Gamepad.current.buttonWest.wasPressedThisFrame
            || Gamepad.current.buttonNorth.wasPressedThisFrame
            || Gamepad.current.buttonEast.wasPressedThisFrame;

        if (keyPressed || buttonPressed)
        {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}
