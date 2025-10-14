using UnityEngine;

public class EnemyConfiguration : MonoBehaviour
{
     public float horizontalTurnSpeed;
    public float verticalLookSpeed;
    public float minYLookAngle;
    public float maxYLookAngle;
    public float forwardMoveSpeed;
    public float backwardMoveSpeed;
    public float sidewaysMoveSpeed;
    public float sprintSpeedMultiplier;
    public float jumpForce;
    public float swingJumpDirectionalForce;
    public float linearVelocityDrag;

    public float jetpackUpwardsForce;
    public float jetpackForwardForce;
    public float jetpackFuelSeconds;
    public AnimationCurve jetpackFuelResponseCurve;
}
