using System.Collections.Generic;
using UnityEngine;

public class EnemyEffects
{

    private List<IEnemyModifierEffect> activeEffects = new();
    public void RegisterActiveEffect(IEnemyModifierEffect effect)
    {
        if (!activeEffects.Contains(effect))
        {
            activeEffects.Add(effect);
        }
    }
    public void UnregisterActiveEffect(IEnemyModifierEffect effect)
    {
        if (activeEffects.Contains(effect))
        {
            activeEffects.Remove(effect);
        }
    }

    public float GetMovementSpeedMultiplier()
    {
        float multiplier = 1f;
        foreach (var effect in activeEffects)
        {
            if (effect is IEnemyMovespeedEffect speedEffect)
            {
                multiplier *= speedEffect.speedMultiplier;
            }
        }
        return multiplier;
    }
     public bool GetIsTouchingGround()
    {
        foreach (var effect in activeEffects)
        {
            if (effect is EnemyGroundEffect groundEffect)
            {
                return groundEffect.touchingGround;
            }
        }
        return false;
    }
    public bool GetIsJumping()
    {
        foreach (var effect in activeEffects)
        {
            if (effect is EnemyJumpingEffect)
            {
                return true;
            }
        }
        return false;
    }


    public interface IEnemyModifierEffect
    {

    }

    public interface IEnemyMovespeedEffect : IEnemyModifierEffect
    {
        float speedMultiplier { get; }
    }

    public class EnemyMovementSpeedEffect : IEnemyModifierEffect
    {
        public float speedMultiplier { get; set; } = 1f;
    }

    public class EnemyGroundEffect : IEnemyModifierEffect
    {
        public bool touchingGround { get; set; } = false;
    }

    public class EnemyJumpingEffect : IEnemyModifierEffect
    {
        //marker class
    }
}
