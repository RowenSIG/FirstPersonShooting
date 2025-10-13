using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;
using static Logging;

public class Player : NetworkBehaviour
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

    private PlayerEffects modifierEffects = new PlayerEffects();
    public PlayerEffects ModifierEffects => modifierEffects;

    private List<PlayerComponentControls> allControls;

    private bool IsLocal => Object.HasInputAuthority;

    public override void Spawned()
    {
        base.Spawned();
        if(IsLocal == false)
        {
            playerCamera.enabled = false;
        }
    }
    public void Setup(PlayerConfiguration config)
    {
        var found = gameObject.GetComponentsInChildren<PlayerComponentControls>();
        allControls = new List<PlayerComponentControls>(found);

        foreach (var control in allControls)
        {
            control.Setup(config, this);
        }

        if(IsLocal == false)
        {
            playerCamera.enabled = false;
        }
    }

    public void InitialiseInput(InputDevice[] devices, int playerIndex, InputSystem_Actions inputControls)
    {
        this.playerIndex = playerIndex;

        if (IsLocal == false)
            return;

        input.Initialise(devices, playerIndex, inputControls);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private struct InputCache
    {
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        public bool fire;
        public bool altFire;
        public bool reload;
        public bool sprint;
        public bool previousWeapon;
        public bool nextWeapon;
        public float scrollDirection;
    }
    InputCache cachedInput;

    public void Update()
    {
        if (IsLocal == false)
            return;

        if (input.MoveAction == null)
            return;

        cachedInput = new InputCache()
        {
            move = input.MoveAction.ReadValue<Vector2>(),
            look = input.LookAction.ReadValue<Vector2>(),
            jump = input.JumpAction.IsPressed(),
            fire = input.FireAction.IsPressed(),
            altFire = input.AltFireAction.IsPressed(),
            reload = input.ReloadAction.WasPressedThisFrame(),
            sprint = input.SprintAction.IsPressed(),
            previousWeapon = input.PrevWeaponAction.WasPressedThisFrame(),
            nextWeapon = input.NextWeaponAction.WasPressedThisFrame(),
            scrollDirection = input.ScrollWeaponAction != null ? input.ScrollWeaponAction.ReadValue<Vector2>().y : 0f,
        };

        
        foreach (var control in allControls)
        {
            control.UpdateFireInput(cachedInput.fire, cachedInput.altFire, cachedInput.reload);
            control.UpdatePrevNextInput(cachedInput.previousWeapon, cachedInput.nextWeapon, cachedInput.scrollDirection);
        }
    }
    
    public override void FixedUpdateNetwork()
    {
        if (IsLocal == false)
            return;

        foreach (var control in allControls)
        {
            control.UpdateLookInput(cachedInput.look);
            control.UpdateMoveInput(cachedInput.move, cachedInput.jump, cachedInput.sprint);
            // control.UpdateFireInput(cachedInput.fire, cachedInput.altFire, cachedInput.reload);
            // control.UpdatePrevNextInput(cachedInput.previousWeapon, cachedInput.nextWeapon, cachedInput.scrollDirection);
            control.UpdateFixedPhysics();
        }
    }

    void FixedUpdateX()
    {
        if (IsLocal == false)
            return;

        foreach (var control in allControls)
        {
            control.UpdateFixedPhysics();
        }
    }

    public void SetNewPlayerUp(Vector3 up)
    {
        if (Vector3.Dot(up, playerUp) > 0.999f)
            return;

        Log($"[PlayerComponentControls] SetNewPlayerUp [{up}]");
        playerUp = up;
        OnNewPlayerUp.Invoke();

    }

    public bool CanJump()
    {
        return modifierEffects.GetIsTouchingGround();
    }
}
