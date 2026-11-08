using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class Pushable : MonoBehaviour
{
    [Header("Push Settings")]
    [Tooltip("How strong the push feels. Adjust based on mass.")]
    public float pushPower = 2f;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Prevent rotation so the object slides instead of tumbles
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // Called by the player when colliding (if using CharacterController) or directly via other scripts
    public void Push(Vector3 direction)
    {
        if (rb == null || rb.isKinematic) return;
        Vector3 pushDir = new Vector3(direction.x, 0, direction.z).normalized;
        rb.linearVelocity = pushDir * pushPower;
    }
}
