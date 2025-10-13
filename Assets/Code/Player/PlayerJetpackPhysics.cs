using UnityEngine;
using static Logging;

public class PlayerJetpackPhysics : PlayerComponentControls
{
    private float remainingFuel = 0f;

    private bool postJumpNoPressPeriod = false;

    public override void UpdateMoveInput(Vector2 moveInput, bool jumpInput, bool sprintInput)
    {
        //first, are we recharged?
        if (player.CanJump())
        {
            remainingFuel = config.jetpackFuelSeconds;
            postJumpNoPressPeriod = true;
        }
        else
        {

            if (postJumpNoPressPeriod == false && jumpInput && remainingFuel > 0f)
            {
                var upwardsForce = Vector3.up * config.jetpackUpwardsForce * PlayerFixedDT;
                var forwardsForce = player.transform.forward * config.jetpackForwardForce * PlayerFixedDT * Mathf.Clamp01(moveInput.y);
                var forceMultiplier = config.jetpackFuelResponseCurve.Evaluate(1f - (remainingFuel / config.jetpackFuelSeconds));
                var totalForce = (upwardsForce + forwardsForce) * forceMultiplier;
                player.Body.AddForce(totalForce, ForceMode.VelocityChange);
                remainingFuel -= PlayerFixedDT;
            }
            
            if (postJumpNoPressPeriod && jumpInput == false)
            {
                postJumpNoPressPeriod = false;
            }
        }

    }
}
