using UnityEngine;

namespace UponHeavyTides.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputReader))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float movingSpeed = 3f;
        [SerializeField] private float acceleration = 6f;
        [SerializeField] private float rotationSpeed = 8f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 0.9f;
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private PlayerInputReader input;
        private Vector3 horizontalVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputReader>();
        }

        private void Update()
        {
            ApplyHorizontalMovement();
            ApplyVerticalMovement();
            characterController.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            RotateTowardsMovement();
        }

        private void ApplyHorizontalMovement()
        {
            Vector2 move = input.Move;
            Vector3 desired = new Vector3(move.x, 0f, move.y) * movingSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, desired, acceleration * Time.deltaTime);
        }

        private void ApplyVerticalMovement()
        {
            if (characterController.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            if (characterController.isGrounded && input.JumpPressed)
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            verticalVelocity += gravity * Time.deltaTime;
        }

        private void RotateTowardsMovement()
        {
            if (horizontalVelocity.sqrMagnitude <= 0.05f) return;
            Quaternion target = Quaternion.LookRotation(horizontalVelocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }
}