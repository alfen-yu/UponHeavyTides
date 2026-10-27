using UnityEngine;
using UnityEngine.InputSystem;

namespace UponHeavyTides.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference jumpAction;

        public Vector2 Move => moveAction.action.ReadValue<Vector2>();
        public bool JumpPressed => jumpAction.action.WasPressedThisFrame();

        private void OnEnable()
        {
            moveAction.action.Enable();
            jumpAction.action.Enable();
        }

        private void OnDisable()
        {
            moveAction.action.Disable();
            jumpAction.action.Disable();
        }
    }
}