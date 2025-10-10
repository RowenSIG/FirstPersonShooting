using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static Logging;

public class Player : MonoBehaviour
{
    [SerializeField]
    private PlayerInput input;

    [SerializeField]
    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;

    [SerializeField]
    private Rigidbody body;
    public Rigidbody Body => body;

    private Vector3 playerUp = Vector3.up;
    public Vector3 PlayerUp => playerUp;

    private int playerIndex;
    public int PlayerIndex => playerIndex;

    public Action OnNewPlayerUp = () => { };

    public bool RopeSwinging { get; private set; }

    private List<PlayerComponentControls> allControls;
    public void Setup(PlayerConfiguration config)
    {
        var found = gameObject.GetComponentsInChildren<PlayerComponentControls>();
        allControls = new List<PlayerComponentControls>(found);

        foreach (var control in allControls)
        {
            control.Setup(config, this);
        }
    }

    public void InitialiseInput(InputDevice[] devices, int playerIndex, InputSystem_Actions inputControls)
    {
        this.playerIndex = playerIndex;
        input.Initialise(devices, playerIndex, inputControls);

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 move = input.MoveAction.ReadValue<Vector2>();
        Vector2 look = input.LookAction.ReadValue<Vector2>();
        bool jump = input.JumpAction.WasPerformedThisFrame();
        bool fire = input.FireAction.IsPressed();
        bool altFire = input.AltFireAction.IsPressed();
        bool reload = false;

        bool previousWeapon = input.PrevWeaponAction.WasPressedThisFrame();
        bool nextWeapon = input.NextWeaponAction.WasPressedThisFrame();

        // Log($"[Player] Update move[{move}] look[{look}] jump[{jump}] fire[{fire}]");

        foreach (var control in allControls)
        {
            control.UpdateLookInput(look);
            control.UpdateMoveInput(move, jump);
            control.UpdateFireInput(fire, altFire, reload);
            control.UpdatePrevNextInput(previousWeapon, nextWeapon,  input.ScrollWeaponAction.ReadValue<Vector2>().y );
        }
    }

    void FixedUpdate()
    {

        foreach (var control in allControls)
        {
            control.UpdateFixedPhysics();
        }
    }

    public void SetNewPlayerUp(Vector3 up)
    {
        canJump = true;
        if (Vector3.Dot(up, playerUp) > 0.999f)
            return;

        Log($"[PlayerComponentControls] SetNewPlayerUp [{up}]");
        playerUp = up;
        OnNewPlayerUp.Invoke();

    }

    public void FloorDetected(Vector3 floorNormal)
    {
        canJump = true;
    }
    private bool canJump = true;
    public bool CanJump()
    {
        return canJump;
    }
    public void Jump()
    {
        canJump = false;
    }
}
