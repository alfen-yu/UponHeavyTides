using UnityEngine;

namespace UponHeavyTides.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float movingSpeed = 4.5f;
        [SerializeField] private float acceleration = 12f;
        [SerializeField] private float rotationSpeed = 10f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 1.2f;
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
            // Gather input from Arrow Keys and WASD / Input System
            float horizontal = 0f;
            float vertical = 0f;

            // Direct arrow key input
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) vertical += 1f;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) vertical -= 1f;
            if (Input.GetKey(KeyCode.LeftArrow)) horizontal -= 1f;
            if (Input.GetKey(KeyCode.RightArrow)) horizontal += 1f;

            // If Input System reader provides values, combine them safely
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
                    // Ignore Input System binding issues during tests
                }
            }

            Vector2 inputDir = Vector2.ClampMagnitude(new Vector2(horizontal, vertical), 1f);

            // Calculate movement relative to Camera view
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
            bool jumpPressed = Input.GetKeyDown(KeyCode.Space);
            if (input != null)
            {
                try
                {
                    if (input.JumpPressed) jumpPressed = true;
                }
                catch
                {
                    // Ignore
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
