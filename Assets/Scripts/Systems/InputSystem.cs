using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

// TODO: Forzar que se actualice este sistema antes que el que mueva al jugador.
//[DisableAutoCreation]
[AlwaysUpdateSystem]
public class InputSystem : SystemBase/*, InputActions.IPlayerActionsActions*/ // No es necesario implementar esta interfaz porque no vamos a usar los eventos.
{
    // Cada uno de estos objetos define una entrada del usuario, habra una por mando.
    InputActions inputActions, inputActions2;
    EntityQuery characterControllerInputDataQuery;
    EntityQuery playerQuery;

    // Variables auxiliares para luego pasarselas al componente que pertenece a cada entidad (jugador1 y jugador2).
    Vector2 player1Movement;
    Vector2 player1LookDirection;
    bool isPlayer1Shooting;

    Vector2 player2Movement;
    Vector2 player2LookDirection;
    bool isPlayer2Shooting;

    protected override void OnStartRunning()
    {

        inputActions?.Enable();
        inputActions2?.Enable();
    }

    protected override void OnStopRunning()
    {
        inputActions?.Disable();
        inputActions2?.Disable();
    }

    // [System.Obsolete]
    protected override void OnCreate()
    {
        // Diferenciamos si estamos en la escena de 1 jugador o en la de 2.
        if (SceneManager.GetActiveScene().name == "12Jugador") // TODO: Este valor esta para hacer pruebas.
        {
            //Definimos una instancia que manejara los inputs del jugador.
            inputActions = new InputActions();
            //inputActions.PlayerActions.SetCallbacks(this);

            // Recogemos los metodos de entrada del jugador.
            Gamepad gamepad = Gamepad.all.Count > 0 ? Gamepad.all[0] : null; // Si tiene mando lo recogemos.
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            // El jugador dispondra de estos tres metodos para interactuar.
            inputActions.devices = new InputDevice[] { gamepad, keyboard, mouse };

            BindPlayer1Actions();
        }
        else
        {
            // Definimos las instancias que manejaran los inputs de cada jugador. 
            inputActions = new InputActions();
            //inputActions.PlayerActions.SetCallbacks(this);

            inputActions2 = new InputActions();
            //inputActions2.PlayerActions.SetCallbacks(this);

            /*
                Si hay dos mandos asignamos un mando a cada jugador y damos la opcion
                al player1 de seguir usando el teclado y raton.
            */
            if (Gamepad.all.Count >= 2)
            {
                inputActions.devices = new InputDevice[] { Gamepad.all[0], Keyboard.current, Mouse.current };
                inputActions2.devices = new InputDevice[] { Gamepad.all[1] };
            }
            else
            {
                inputActions.devices = new InputDevice[] { Gamepad.all[0] };
                inputActions2.devices = new InputDevice[] { Keyboard.current, Mouse.current };
            }

            BindPlayer1Actions();

            BindPlayer2Actions();

        }

        // TODO: Modificar esto para que tenga en cuenta a los dos jugadores.
        characterControllerInputDataQuery = GetEntityQuery(typeof(CharacterControllerInputData));
        playerQuery = GetEntityQuery(typeof(PlayerTag));

    }

    [BurstDiscard]
    protected override void OnUpdate()
    {
        // Formas de implementacion alternativas.
        #region
        // TODO: Esto de que haya una entidad por ahi flotando no me mola, deberiamos coger el componenData del personaje y meterle los datos a el.
        //if (characterControllerInputDataQuery.CalculateEntityCount() == 0)
        //{
        //    EntityManager.CreateEntity(typeof(CharacterControllerInputData));
        //}

        //characterControllerInputDataQuery.SetSingleton(new CharacterControllerInputData() { 
        //    Movement = playerMovement,
        //    Looking = playerLookDirection
        //});

        //var localPlayerMovement = (float2)playerMovement;
        //var localPlayerLookDirection = (float2)playerLookDirection;
        //Entities.WithAll<PlayerTag>().ForEach((ref CharacterControllerInputData playerInputData) =>
        //{
        //    playerInputData.Movement = localPlayerMovement;
        //    playerInputData.Looking = localPlayerLookDirection;
        //}).Schedule();
        #endregion 
        NativeArray<CharacterControllerInputData> auxCharactersControllerData = new NativeArray<CharacterControllerInputData>(2, Allocator.Temp);

        CharacterControllerInputData inputData = new CharacterControllerInputData();
        inputData.Movement = player1Movement;
        inputData.Looking = player1LookDirection;
        inputData.shooting = isPlayer1Shooting;

        auxCharactersControllerData[0] = inputData;

        inputData.Movement = player2Movement;
        inputData.Looking = player2LookDirection;
        inputData.shooting = isPlayer2Shooting;

        auxCharactersControllerData[1] = inputData;
        // Generamos una estructura que contenga todos los datos relativos a los input del jugador.
        //CharacterControllerInputData[] auxCharactersControllerData = { new CharacterControllerInputData(), new CharacterControllerInputData() } ;
        // Asignamos todos los datos relativos a los inputs del jugador.

        Entities.WithAll<PlayerTag>().ForEach((ref CharacterControllerInputData characterInputData, in PlayerTag playerTag) =>
        {
            
            if (playerTag.playerId == 1)
            {
                characterInputData = auxCharactersControllerData[0];
            }
            else if (playerTag.playerId == 2)
            {
                characterInputData = auxCharactersControllerData[1];
            }
        }).Run(); // Lo ejecutamos en el hilo principal.


        auxCharactersControllerData.Dispose();
        //EntityManager.SetComponentData<CharacterControllerInputData>(playerAuxEntity, auxCharactersControllerData[0]); // TODO: Al insertar aqui los datos machacamos el cannon?.
    }

    void BindPlayer1Actions()
    {
        inputActions.PlayerActions.Move.performed += ctx => player1Movement = ctx.ReadValue<Vector2>();
        inputActions.PlayerActions.Look.performed += ctx => player1LookDirection = ctx.ReadValue<Vector2>();
        inputActions.PlayerActions.Shoot.performed += ctx => isPlayer1Shooting = ctx.performed;

        inputActions.PlayerActions.Move.canceled += ctx => player1Movement = ctx.ReadValue<Vector2>();
        inputActions.PlayerActions.Look.canceled += ctx => player1LookDirection = ctx.ReadValue<Vector2>();
        inputActions.PlayerActions.Shoot.canceled += ctx => isPlayer1Shooting = !ctx.canceled;
    }

    void BindPlayer2Actions()
    {
        inputActions2.PlayerActions.Move.performed += ctx => player2Movement = ctx.ReadValue<Vector2>();
        inputActions2.PlayerActions.Look.performed += ctx => player2LookDirection = ctx.ReadValue<Vector2>();
        inputActions2.PlayerActions.Shoot.performed += ctx => isPlayer2Shooting = ctx.performed;

        inputActions2.PlayerActions.Move.canceled += ctx => player2Movement = ctx.ReadValue<Vector2>();
        inputActions2.PlayerActions.Look.canceled += ctx => player2LookDirection = ctx.ReadValue<Vector2>();
        inputActions2.PlayerActions.Shoot.canceled += ctx => isPlayer2Shooting = !ctx.canceled;
    }

    // Forma alternativa implementando la interfaz.
    // Cogemos los inputs por mando y los asignamos a las variables locales, estas variables se escribiran en la strcutura que conforma los datos del jugador y que usaran los sistemas.
    //void InputActions.IPlayerActionsActions.OnMove(InputAction.CallbackContext context) => playerMovement = context.ReadValue<Vector2>();
    //void InputActions.IPlayerActionsActions.OnLook(InputAction.CallbackContext context) => playerLookDirection = context.ReadValue<Vector2>();
    //void InputActions.IPlayerActionsActions.OnShoot(InputAction.CallbackContext context) => isPlayerShooting = context.performed;

}
