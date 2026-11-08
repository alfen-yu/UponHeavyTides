using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [Header("Target Tracking")]
    public Transform target;
    public float distance = 7.5f;
    public float height = 4.5f;
    public float transition = 6f;

    [Header("Camera Rotation Controls (A / D)")]
    [Tooltip("Turn speed in degrees per second")]
    public float turnSpeed = 90f;
    private float currentYaw = 0f;

    private void Start()
    {
        FindPlayerTarget();
        if (target != null)
        {
            Vector3 dir = (transform.position - target.position);
            dir.y = 0;
            if (dir.sqrMagnitude > 0.01f)
            {
                currentYaw = Quaternion.LookRotation(-dir).eulerAngles.y;
            }
        }
    }

    private void FindPlayerTarget()
    {
        if (target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            FindPlayerTarget();
            if (target == null) return;
        }

        // User requested: "press A to turn camera right and press D to turn camera left"
        if (Input.GetKey(KeyCode.A))
        {
            currentYaw += turnSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            currentYaw -= turnSpeed * Time.deltaTime;
        }

        Quaternion rotation = Quaternion.Euler(0f, currentYaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
        Vector3 desiredPosition = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, transition * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.2f);
    }

    public Quaternion GetCameraYawRotation()
    {
        return Quaternion.Euler(0f, currentYaw, 0f);
    }
}
