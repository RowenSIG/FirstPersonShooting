using UnityEngine;

public class EnemyVisualsControls : EnemyComponentControls
{
      [SerializeField]
    private Rigidbody rigidBody;

    [SerializeField]
    private Transform camLookTransform;


    public override void UpdateLookInput(Vector2 input)
    {
        float x = input.x;
        //jitter!?
        if (Mathf.Abs(x) < 0.1f)
        {
            x = 0;
        }

        //horizontal turning
        var up = EnemyUp;
        var rotX = Quaternion.AngleAxis(x * config.horizontalTurnSpeed * EnemyDT, up);
        var look = transform.rotation;
        look = rotX * look;
        transform.rotation = look;

        EnsureUpwardsVector();

      
    }

    private void EnsureUpwardsVector()
    {
        var up = EnemyUp;
        //compute our look along the plane our up is normal to:
        Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, up).normalized;

        transform.rotation = Quaternion.LookRotation(projectedForward, up);
        rigidBody.rotation = transform.rotation;


    }
}
