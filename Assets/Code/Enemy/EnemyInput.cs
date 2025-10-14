using ExitGames.Client.Photon.StructWrapping;
using UnityEngine;

public class EnemyInput : MonoBehaviour
{

    public Vector2 Move;
    public Vector2 Look;
    public bool Jump;
    public bool Fire;
    public bool AltFire;
    public bool Reload;
    public bool Sprint;
    public bool PrevWeapon;
    public bool NextWeapon;

    public void Update()
    {
        //we're going to move towards the player:

        var player = PlayerManager.Instance.GetLocalPlayer();
        if (player == null)
            return;

        var offsetToPlayer = player.transform.position - transform.position;
        var directionToPlayer = offsetToPlayer.normalized;
        //we turn towards that direction:
        var angle = Vector3.SignedAngle(transform.forward, directionToPlayer, Vector3.up);
        if (angle > 5f)
            Look.x = 1f;
        else if (angle < 5f)
            Look.x = -1f;
        else
            Look.x = 0f;
            
        //we move twoards the player:
        if(Mathf.Abs(angle) < 90f)
        {
            Move.y = 1f;
        }
    }

}
