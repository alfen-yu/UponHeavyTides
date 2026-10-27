using UnityEngine;
using UponHeavyTides.Interaction;

namespace UponHeavyTides.Player
{
    public class PlayerPusher : MonoBehaviour
    {
        [SerializeField] private float pushSpeed = 3f;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!hit.collider.TryGetComponent(out Pushable pushable)) return;
            if (hit.moveDirection.y < -0.3f) return;

            Vector3 direction = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z).normalized;
            pushable.Push(direction, pushSpeed);
        }
    }
}