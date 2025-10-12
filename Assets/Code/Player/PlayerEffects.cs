using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects
{

    private List<IPlayerModifierEffect> activeEffects = new();

    public void RegisterActiveEffect(IPlayerModifierEffect effect)
    {
        if (!activeEffects.Contains(effect))
        {
            activeEffects.Add(effect);
        }
    }

    public void UnregisterActiveEffect(IPlayerModifierEffect effect)
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
            if (effect is IPlayerMovespeedEffect speedEffect)
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
            if (effect is PlayerGroundEffect groundEffect)
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
            if (effect is PlayerJumpingEffect)
            {
                return true;
            }
        }
        return false;
    }

    public interface IPlayerModifierEffect
    {
    }

    public interface IPlayerMovespeedEffect : IPlayerModifierEffect
    {
        float speedMultiplier { get; }
    }

    public class PlayerMovementSpeedEffect : IPlayerMovespeedEffect
    {
        public float speedMultiplier { get; set; } = 1f;
    }

    public class PlayerGroundEffect : IPlayerModifierEffect
    {
        public bool touchingGround { get; set; } = false;
    }
    public class PlayerJumpingEffect : IPlayerModifierEffect
    {
        //marker class
    }
}