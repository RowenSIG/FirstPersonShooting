using System.Collections.Generic;
using Fusion;
using UnityEngine;

public abstract class PlayerWeapon : NetworkBehaviour
{
    protected Player player;


    [SerializeField]
    private List<GameObject> visualsObjects;

    [Networked] public bool IsVisible {get;set;}
    
    public virtual void Setup(Player player)
    {
        this.player = player;
    }

    public override void Render()
    {
        foreach (var obj in visualsObjects)
        {
            obj.EnsureActive(IsVisible);
        }
    }

    public virtual void UpdateWeapon(float deltaTime, bool leftFire, bool rightFire, bool reloadButton) { }
    
}

