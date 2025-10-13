using UnityEngine;

public class SmashBlock : MonoBehaviour
{
    
    private float massOnAwake = 1f;

    public void GoPhysical(Rigidbody rigidbody)
    {
        rigidbody.mass = massOnAwake;
    }
}
