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

        public void Push(Vector3 direction, float pusherSpeed)
        {
            Vector3 velocity = direction * (pusherSpeed * pushSpeedFactor);
            velocity.y = body.linearVelocity.y;
            body.linearVelocity = velocity;
        }
    }
}