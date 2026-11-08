using UnityEngine;

namespace UponHeavyTides.Interaction
{
    [RequireComponent(typeof(Rigidbody))]
    public class Pushable : MonoBehaviour
    {
        [Header("Push Dynamics")]
        [SerializeField, Range(0.1f, 2f)] private float pushSpeedFactor = 0.8f;
        [SerializeField] private float minimumPushSpeed = 2.5f;

        private Rigidbody body;
        private float currentPusherSpeed = 3f;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            if (body != null)
            {
                body.isKinematic = false;
                body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        public void SetPushSpeed(float speed)
        {
            currentPusherSpeed = speed;
        }

        public void Push(Vector3 direction)
        {
            if (body == null || body.isKinematic) return;

            Vector3 pushDir = new Vector3(direction.x, 0f, direction.z).normalized;
            float force = Mathf.Max(currentPusherSpeed * pushSpeedFactor, minimumPushSpeed);

            Vector3 newVel = pushDir * force;
            newVel.y = body.linearVelocity.y;
            body.linearVelocity = newVel;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<UponHeavyTides.Player.PlayerMovement>() != null)
            {
                Vector3 pushDir = (transform.position - collision.transform.position);
                pushDir.y = 0;
                if (pushDir.sqrMagnitude > 0.01f)
                {
                    Push(pushDir.normalized);
                }
            }
        }
    }
}
