using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField]
    private float recoilDisplacementMagnitude = 0.05f; // meters backwards
    [SerializeField]
    private Vector3 recoilDirectionMagnitude = new Vector3(5f, 2f, 0f); // pitch, yaw, roll
    [SerializeField]
    private float recoilDuration = 0.2f;
    [SerializeField]
    private AnimationCurve recoilCurve;

    private Vector3 initialLocalPosition;
    private Quaternion initialRotation;
    private float recoilTimer = 0f;
    private float recoilMagnitude = 0f;
    private Vector3 recoilTargetOffset;

    void Start()
    {
        initialRotation = transform.localRotation;
        initialLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (recoilTimer > 0f)
        {
            float elapsed = recoilDuration - recoilTimer;
            float normalizedTime = Mathf.Clamp01(elapsed / recoilDuration);
            float ease = recoilCurve != null ? recoilCurve.Evaluate(normalizedTime) : Mathf.SmoothStep(1f, 0f, normalizedTime);
            Vector3 offset = Vector3.Lerp(recoilTargetOffset, Vector3.zero, normalizedTime) * ease;
            Quaternion targetRot = initialRotation * Quaternion.Euler(offset);
            transform.localRotation = targetRot;
            // Displacement backwards (local -Z)
            float displacement = Mathf.Lerp(recoilDisplacementMagnitude, 0f, normalizedTime) * ease;
            transform.localPosition = initialLocalPosition - Vector3.forward * displacement;
            recoilTimer -= Time.deltaTime;
            if (recoilTimer <= 0f)
            {
                transform.localRotation = initialRotation;
                transform.localPosition = initialLocalPosition;
            }
        }
        else
        {
            transform.localRotation = initialRotation;
            transform.localPosition = initialLocalPosition;
        }
    }

    public void DoRecoil(float magnitude)
    {
        recoilTimer = recoilDuration;
        recoilMagnitude = magnitude;
        // Generate a new random offset and blend it with the current offset for smooth stacking
        Vector3 newOffset = new Vector3(
           Random.value * recoilDirectionMagnitude.x * recoilMagnitude,
           Random.value * recoilDirectionMagnitude.y * recoilMagnitude,
           Random.value * recoilDirectionMagnitude.z * recoilMagnitude
        );
        recoilTargetOffset = Vector3.Lerp(recoilTargetOffset, newOffset, 0.7f);
    }
}
