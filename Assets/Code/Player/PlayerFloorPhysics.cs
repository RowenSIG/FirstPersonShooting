using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Logging;

public class PlayerFloorPhysics : PlayerComponentControls
{
    private float floorNormalThreshold = 0.5f;
    private List<ContactPoint> contactPoints = new List<ContactPoint>();
    private PlayerEffects.IPlayerModifierEffect activeGroundEffect = null;

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
                activeGroundEffect = new PlayerEffects.PlayerGroundEffect() { touchingGround = true};
                player.ModifierEffects.RegisterActiveEffect(activeGroundEffect);
            }
        }
        else
        {
            if (activeGroundEffect != null)
            {
                player.ModifierEffects.UnregisterActiveEffect(activeGroundEffect);
                activeGroundEffect = null;
            }
        }
    }

    private void ApplyAntiRampAcceleration(Vector3 contactNormal)
    {
        //it should be easy, get the sideways force resulting from our ramp and push back.
        var perp = Vector3.Cross(contactNormal, Vector3.up);
        var rampTangentDirection = Vector3.Cross(perp, contactNormal);
        player.Body.AddForce(rampTangentDirection * Game.GRAVITY_ACCELERATION , ForceMode.Acceleration); //we sort of slide...

#if UNITY_EDITOR
        rampPerp = perp;
        rampDir = rampTangentDirection;
#endif
    }

    private void OnCollisionStay(Collision collision)
    {
        if (player != null && player.HasInputAuthority)
        {
            contactPoints.AddRange(collision.contacts);
        }
    }

#if UNITY_EDITOR
    private Vector3 rampDir = Vector3.zero;
    private Vector3 rampPerp = Vector3.zero;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var contact in contactPoints)
        {
            Gizmos.DrawWireCube(contact.point, Vector3.one * 0.1f);
            Gizmos.DrawLine(contact.point, contact.point + contact.normal * 2f);
        }

        if (contactPoints.Count > 0)
        {
            Gizmos.DrawLine(contactPoints[0].point, contactPoints[0].point + rampDir * 10f);
            Gizmos.color = Color.orange;
            Gizmos.DrawLine(contactPoints[0].point, contactPoints[0].point + rampPerp * 10f);
        }
    }
#endif

}
