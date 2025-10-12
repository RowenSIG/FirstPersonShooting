using UnityEngine;
using static Logging;

public class PlayerPhysicsControls : PlayerComponentControls
{
    private Rigidbody Body => player.Body;

    private bool sprinting = false;
    private PlayerEffects.IPlayerModifierEffect activeSprintEffect = null;
    private PlayerEffects.IPlayerModifierEffect activeJumpingEffect = null;

    public override void UpdateMoveInput(Vector2 moveInput, bool jumpInput, bool sprintInput)
    {
        float sprintSpeedMultiplier = UpdateSprintInput(moveInput, sprintInput);

        var forwardStrength = config.forwardMoveSpeed * sprintSpeedMultiplier;
        var backwardStrength = config.backwardMoveSpeed;
        var sidewaysStrength = config.sidewaysMoveSpeed;


        var forwardForce = Mathf.Clamp(moveInput.y, 0f, 1f) * PlayerDT * forwardStrength;
        var backwardForce = Mathf.Clamp(moveInput.y, -1f, 0f) * PlayerDT * backwardStrength;

        var sidewaysForce = moveInput.x * PlayerDT * sidewaysStrength;

        var force = new Vector3(sidewaysForce, 0, forwardForce + backwardForce);
        Body.AddRelativeForce(force, ForceMode.VelocityChange);


        //jump?
        if (player.ModifierEffects.GetIsTouchingGround())
        {
            if (jumpInput && activeJumpingEffect == null)
            {
                var jumpUpwardsForce = Vector3.up * config.jumpForce;

                Body.AddRelativeForce(jumpUpwardsForce, ForceMode.VelocityChange);

                activeJumpingEffect = new PlayerEffects.PlayerJumpingEffect() { };
                player.ModifierEffects.RegisterActiveEffect(activeJumpingEffect);
                Log("[PlayerPhysicsControls] jumping");
            }
            else if (jumpInput == false)
            {
                //we've let go of jump, so we can jump again when we hit the ground
                if (activeJumpingEffect != null)
                {
                    player.ModifierEffects.UnregisterActiveEffect(activeJumpingEffect);
                    activeJumpingEffect = null;
                }
            }
        }
    }

    private float UpdateSprintInput(Vector2 moveInput, bool sprintInput)
    {
        bool shouldBeSprinting = sprintInput && moveInput.y > 0.1f;
        if (sprinting == false && shouldBeSprinting)
        {
            activeSprintEffect = new PlayerEffects.PlayerMovementSpeedEffect() { speedMultiplier = config.sprintSpeedMultiplier };
            player.ModifierEffects.RegisterActiveEffect(activeSprintEffect);
            sprinting = true;
        }
        else if (sprinting && shouldBeSprinting == false)
        {
            player.ModifierEffects.UnregisterActiveEffect(activeSprintEffect); 
            activeSprintEffect = null;
            sprinting = false;

        }

        if (sprinting)
        {
            return player.ModifierEffects.GetMovementSpeedMultiplier(); 
        }

        return 1f;
    }

    public override void UpdateFixedPhysics()
    {
        var gravity = -1f * Game.GRAVITY_ACCELERATION * PlayerUp;
        Body.AddForce(gravity, ForceMode.Acceleration);

        //drag
        var velocity = Body.linearVelocity;
        //don't drag falling
        var fallingComponent = Vector3.Dot(PlayerUp, velocity) * PlayerUp.normalized;
        var touchingGround = player.ModifierEffects.GetIsTouchingGround();

        if(touchingGround == false)
        {
            velocity -= fallingComponent;
            velocity *= config.linearVelocityDrag;
            velocity += fallingComponent;
        }
        else
        {
            velocity *= config.linearVelocityDrag;
        }
        Body.linearVelocity = velocity;
    }
}
