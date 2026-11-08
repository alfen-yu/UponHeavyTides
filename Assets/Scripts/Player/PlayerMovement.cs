using UnityEngine;
using UnityEngine.InputSystem;

namespace UponHeavyTides.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float movingSpeed = 5.0f;
        [SerializeField] private float acceleration = 14f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 1.3f;
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private PlayerInputReader input;
        private Vector3 horizontalVelocity;
        private float verticalVelocity;
        private Camera mainCamera;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            input = GetComponent<PlayerInputReader>();
            mainCamera = Camera.main;
        }

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (characterController == null) return;

            ApplyHorizontalMovement();
            ApplyVerticalMovement();
            characterController.Move((horizontalVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);
            RotateTowardsMovement();
        }

        private void ApplyHorizontalMovement()
        {
            float horizontal = 0f;
            float vertical = 0f;

            // Use New Input System Keyboard directly
            var keyboard = Keyboard.current;
            if (keyboard != null)
            {
                if (keyboard.upArrowKey.isPressed || keyboard.wKey.isPressed) vertical += 1f;
                if (keyboard.downArrowKey.isPressed || keyboard.sKey.isPressed) vertical -= 1f;
                if (keyboard.leftArrowKey.isPressed) horizontal -= 1f;
                if (keyboard.rightArrowKey.isPressed) horizontal += 1f;
            }

            // Also check PlayerInputReader if configured
            if (input != null)
            {
                try
                {
                    Vector2 moveInput = input.Move;
                    if (Mathf.Abs(moveInput.x) > 0.05f || Mathf.Abs(moveInput.y) > 0.05f)
                    {
                        horizontal += moveInput.x;
                        vertical += moveInput.y;
                    }
                }
                catch
                {
                    // Safe fallback
                }
            }

            Vector2 inputDir = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

            // Movement relative to Camera orientation
            Vector3 camForward = Vector3.forward;
            Vector3 camRight = Vector3.right;

            if (mainCamera == null) mainCamera = Camera.main;
            if (mainCamera != null)
            {
                camForward = mainCamera.transform.forward;
                camRight = mainCamera.transform.right;
                camForward.y = 0f;
                camRight.y = 0f;
                camForward.Normalize();
                camRight.Normalize();
            }

            Vector3 desired = (camForward * inputDir.y + camRight * inputDir.x) * movingSpeed;
            horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, desired, acceleration * Time.deltaTime);
        }

        private void ApplyVerticalMovement()
        {
            bool jumpPressed = false;
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
            {
                jumpPressed = true;
            }

            if (input != null)
            {
                try
                {
                    if (input.JumpPressed) jumpPressed = true;
                }
                catch
                {
                    // Safe fallback
                }
            }

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }

            if (characterController.isGrounded && jumpPressed)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

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
