using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField]
    private float lifeTime;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > lifeTime)
        {
            Destroy(gameObject);
        }        
    }
}
