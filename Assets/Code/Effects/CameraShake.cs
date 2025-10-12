using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private Vector3 shakeDirectionMagnitude;

    [SerializeField]
    private float shakeFOVMagnitude;

    [SerializeField]
    private AnimationCurve shakeFOVCurve;


    [SerializeField]
    private float shakeDuration = 0.1f;
    
    private Vector3 initialPosition;
    private float initialFOV;
    private Camera cam;
    private float shakeTimer = 0f;
    private float shakeMagnitude = 0f;
    private float shakeFOVAmount = 0f;
    private Vector3 shakeTargetOffset;

    private void Awake()
    {
        initialPosition = transform.localPosition;
        cam = GetComponent<Camera>();
        if (cam != null)
            initialFOV = cam.fieldOfView;
    }

    private void Update()
    {
        if (shakeTimer > 0f)
        {
            float elapsed = shakeDuration - shakeTimer;
            float normalizedTime = Mathf.Clamp01(elapsed / shakeDuration);
            float ease = shakeFOVCurve != null ? shakeFOVCurve.Evaluate(normalizedTime) : Mathf.SmoothStep(1f, 0f, normalizedTime);
            Vector3 offset = Vector3.Lerp(shakeTargetOffset, Vector3.zero, normalizedTime) * ease;
            transform.localPosition = initialPosition + offset;
            // FOV shake
            if (cam != null)
            {
                cam.fieldOfView = initialFOV + shakeFOVAmount * ease;
            }
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                transform.localPosition = initialPosition;
                if (cam != null)
                    cam.fieldOfView = initialFOV;
            }
        }
        else
        {
            transform.localPosition = initialPosition;
            if (cam != null)
                cam.fieldOfView = initialFOV;
        }
    }

    public void DoShake(float magnitude)
    {
        shakeTimer = shakeDuration;
        shakeMagnitude = Mathf.Abs(magnitude);
        shakeFOVAmount = shakeFOVMagnitude * Mathf.Abs(magnitude);
        
        // Generate a new random offset and blend it with the current offset for smooth stacking
        Vector3 newOffset = new Vector3(
            (Random.value - 0.5f) * 2f * shakeDirectionMagnitude.x * shakeMagnitude,
            (Random.value - 0.5f) * 2f * shakeDirectionMagnitude.y * shakeMagnitude,
            (Random.value - 0.5f) * 2f * shakeDirectionMagnitude.z * shakeMagnitude
        );
        // Blend: favor new offset but keep some of the previous for smoothness
        shakeTargetOffset = Vector3.Lerp(shakeTargetOffset, newOffset, 0.7f);
    }
}
