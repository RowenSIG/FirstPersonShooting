using UnityEngine;
[CreateAssetMenu(fileName = "EnemyConfiguration", menuName = "Scriptable Objects/EnemyConfiguration")]

public class EnemyConfiguration : ScriptableObject
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

}
