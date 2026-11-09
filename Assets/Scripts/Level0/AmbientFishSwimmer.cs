using UnityEngine;

namespace UponHeavyTides.Level0
{
    public class AmbientFishSwimmer : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float swimSpeed = 1.8f;
        [SerializeField] private float circleRadius = 3.5f;
        [SerializeField] private float bobHeight = 0.25f;
        [SerializeField] private float bobSpeed = 1.6f;
        [SerializeField] private float wiggleAngle = 10f;
        [SerializeField] private float wiggleSpeed = 7f;

        private Vector3 centerPoint;
        private float angleOffset;
        private float directionMultiplier;

        private void Start()
        {
            centerPoint = transform.position;
            angleOffset = Random.Range(0f, Mathf.PI * 2f);
            directionMultiplier = (Random.value > 0.5f) ? 1f : -1f;
        }

        private void Update()
        {
            float t = Time.time * (swimSpeed / Mathf.Max(0.5f, circleRadius));
            float currentAngle = angleOffset + t * directionMultiplier;

            float x = centerPoint.x + Mathf.Cos(currentAngle) * circleRadius;
            float z = centerPoint.z + Mathf.Sin(currentAngle) * circleRadius;
            float y = centerPoint.y + Mathf.Sin(Time.time * bobSpeed + angleOffset) * bobHeight;

            Vector3 nextPos = new Vector3(x, y, z);
            Vector3 swimDir = (nextPos - transform.position);

            if (swimDir.sqrMagnitude > 0.0001f)
            {
                swimDir.Normalize();
                Quaternion look = Quaternion.LookRotation(swimDir);
                float wiggle = Mathf.Sin(Time.time * wiggleSpeed) * wiggleAngle;
                transform.rotation = look * Quaternion.Euler(0f, wiggle, 0f);
            }

            transform.position = nextPos;
        }

        public void Configure(Vector3 center, float radius, float speed, float depth)
        {
            centerPoint = center;
            circleRadius = radius;
            swimSpeed = speed;
            transform.position = center + Vector3.up * depth;
        }
    }
}
