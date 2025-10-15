using UnityEngine;
using Fusion;
using System.Collections.Generic;

public class Enemy : NetworkBehaviour
{
    
    [SerializeField]
    private EnemyInput input;

    [SerializeField]
    private Rigidbody body;
    public Rigidbody Body => body;

    private Vector3 enemyUp = Vector3.up;
    public Vector3 EnemyUp => enemyUp;

    private EnemyEffects modifierEffects = new EnemyEffects();
    public EnemyEffects ModifierEffects => modifierEffects;

    private List<EnemyComponentControls> allControls;

    private bool IsLocal => Object.HasStateAuthority;

    public float DeltaTime => Runner.DeltaTime;
    public float FixedDeltaTime => Time.fixedDeltaTime;

    [Networked]
    public float hp { get; set; }

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        base.Despawned(runner, hasState);
        Destroy(gameObject);
    }

    public void Setup(EnemyConfiguration config)
    {
        var found = gameObject.GetComponentsInChildren<EnemyComponentControls>();
        allControls = new List<EnemyComponentControls>(found);

        foreach (var control in allControls)
        {
            control.Setup(config, this);
        }

        hp = config.startingHP;
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
    }
    InputCache cachedInput;
    public void Update()
    {
        if (IsLocal == false)
            return;
            
        cachedInput = new InputCache()
        {
            move = input.Move,
            look = input.Look,
            jump = input.Jump,
            fire = input.Fire,
            altFire = input.AltFire,
            reload = input.Reload,
            sprint = input.Sprint,
            previousWeapon = input.PrevWeapon,
            nextWeapon = input.NextWeapon,
        };
   
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    public void FixedUpdate()
    {
        if (IsLocal == false)
            return;

        input.Update();

        foreach (var control in allControls)
        {
            control.UpdateFireInput(cachedInput.fire, cachedInput.altFire, cachedInput.reload);
            control.UpdatePrevNextInput(cachedInput.previousWeapon, cachedInput.nextWeapon);
            control.UpdateLookInput(cachedInput.look);
            control.UpdateMoveInput(cachedInput.move, cachedInput.jump, cachedInput.sprint);
            control.UpdateFireInput(cachedInput.fire, cachedInput.altFire, cachedInput.reload);
            control.UpdatePrevNextInput(cachedInput.previousWeapon, cachedInput.nextWeapon);
            control.UpdateFixedPhysics();
        }

    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }
}
