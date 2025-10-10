using UnityEngine;
using System.Collections.Generic;

public class CameraShake : MonoBehaviour
{
    [SerializeField] 
    private Vector3 shakeDirectionMagnitude;

    [SerializeField]
    private float shakePosMaxSpeed;

    [SerializeField]
    private Vector3 shakeOrientationMagnitude;
    
    private Vector3 initialPosition;

    private struct Shake
    {
        public float timeRemaining;
        public float duration;
        public float magnitude;
    }
    private Queue<Shake> shakes = new Queue<Shake>();

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = initialPosition ;
    }

    public void DoShake(float duration, float magnitude)
    {

    }
}
