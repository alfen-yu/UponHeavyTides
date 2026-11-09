using UnityEngine;

namespace UponHeavyTides.Level0
{
    public class BobbingMotion : MonoBehaviour
    {
        [Header("Bobbing Settings")]
        [SerializeField] private float bobAmplitude = 0.35f;
        [SerializeField] private float bobSpeed = 1.2f;
        [SerializeField] private float tiltAngle = 5f;
        [SerializeField] private float tiltSpeed = 0.8f;

        private Vector3 startPos;
        private Quaternion startRot;
        private float seed;

        private void Start()
        {
            startPos = transform.position;
            startRot = transform.rotation;
            seed = Random.Range(0f, 100f);
        }

        private void Update()
        {
            float yOffset = Mathf.Sin((Time.time + seed) * bobSpeed) * bobAmplitude;
            transform.position = startPos + new Vector3(0f, yOffset, 0f);

            float roll = Mathf.Sin((Time.time + seed) * tiltSpeed) * tiltAngle;
            float pitch = Mathf.Cos((Time.time + seed * 1.3f) * (tiltSpeed * 0.7f)) * (tiltAngle * 0.5f);
            transform.rotation = startRot * Quaternion.Euler(pitch, 0f, roll);
        }
    }
}
