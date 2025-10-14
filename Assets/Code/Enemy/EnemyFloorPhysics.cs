using UnityEngine;
using System.Collections.Generic;

public class EnemyFloorPhysics : EnemyComponentControls
{
    
     private float floorNormalThreshold = 0.5f;
    private List<ContactPoint> contactPoints = new List<ContactPoint>();
    private EnemyEffects.IEnemyModifierEffect activeGroundEffect = null;

    public override void UpdateFixedPhysics()
    {
        //we're going to cancel out 'ramp' physics.

        Vector3 floorContactNormalSum = Vector3.zero;
        int points = 0;
        foreach (var point in contactPoints)
        {
            if (Vector3.Dot(point.normal, Vector3.up) > floorNormalThreshold)
            {
                floorContactNormalSum += point.normal;
                points += 1;
            }
        }

        if (points > 0)
        {
            //we have a floor contact, it appears
            var average = floorContactNormalSum / points;
           ApplyAntiRampAcceleration(average.normalized);
        }

        EnsurePlayerTouchingGround(points > 0);
        contactPoints.Clear();
    }

    private void EnsurePlayerTouchingGround(bool touchingGround)
    {
        if (touchingGround)
        {
            if (activeGroundEffect == null)
            {
                activeGroundEffect = new EnemyEffects.EnemyGroundEffect() { touchingGround = true};
                enemy.ModifierEffects.RegisterActiveEffect(activeGroundEffect);
            }
        }
        else
        {
            if (activeGroundEffect != null)
            {
                enemy.ModifierEffects.UnregisterActiveEffect(activeGroundEffect);
                activeGroundEffect = null;
            }
        }
    }

    private void ApplyAntiRampAcceleration(Vector3 contactNormal)
    {
        //it should be easy, get the sideways force resulting from our ramp and push back.
        var perp = Vector3.Cross(contactNormal, Vector3.up);
        var rampTangentDirection = Vector3.Cross(perp, contactNormal);
        enemy.Body.AddForce(rampTangentDirection * Game.GRAVITY_ACCELERATION , ForceMode.Acceleration); //we sort of slide...

    }

    private void OnCollisionStay(Collision collision)
    {
        if (enemy.HasInputAuthority)
        {
            contactPoints.AddRange(collision.contacts);
        }
    }

}
