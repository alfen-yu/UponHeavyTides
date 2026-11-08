using UnityEngine;

namespace UponHeavyTides.Level0
{
    public class FishSchoolAnimator : MonoBehaviour
    {
        [SerializeField] private float swimSpeed = 1.2f;
        [SerializeField] private float bobHeight = 0.35f;
        [SerializeField] private float bobSpeed = 2.0f;
        [SerializeField] private float rotationWiggle = 15f;

        private Vector3 startPosition;
        private Quaternion startRotation;

        private void Start()
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }

        private void Update()
        {
            float time = Time.time * swimSpeed;
            float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            float xOffset = Mathf.Sin(time) * 0.4f;

            transform.position = startPosition + new Vector3(xOffset, yOffset, 0f);

            float yaw = Mathf.Sin(Time.time * bobSpeed * 1.5f) * rotationWiggle;
            transform.rotation = startRotation * Quaternion.Euler(0f, yaw, 0f);
        }
    }
}
