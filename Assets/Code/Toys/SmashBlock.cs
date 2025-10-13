using UnityEngine;

public class SmashBlock : MonoBehaviour
{
    [SerializeField]
    private float massOnAwake = 1f;

    public void GoPhysical(Rigidbody rigidbody)
    {
        rigidbody.mass = massOnAwake;
    }
}
