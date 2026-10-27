using UnityEngine;

public class CameraTracker: MonoBehaviour {
    // attaches the camera to the target -> player 
    public Transform target;

    // position relative to player
    public Vector3 offset = new Vector3(0f, 4f, -7f);
    public float transition = 6f;

    // runs after update
    void LateUpdate()
    {
        if (target == null) return;

        Vector3 cameraChange = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, cameraChange, transition * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up);
    }
}