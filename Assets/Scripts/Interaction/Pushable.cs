using UnityEngine;

namespace UponHeavyTides.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : MonoBehaviour
    {
        [SerializeField, Range(0.1f, 1f)] private float pushSpeedFactor = 0.5f;

        private Rigidbody body;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
        }

        private float currentPusherSpeed = 1f;

        public void SetPushSpeed(float speed)
        {
            currentPusherSpeed = speed;
        }

        public void Push(Vector3 direction)
        {
            Vector3 velocity = direction * (currentPusherSpeed * pushSpeedFactor);
            velocity.y = body.linearVelocity.y;
            body.linearVelocity = velocity;
        }
    }
}