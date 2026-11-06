using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        private InputAction moveAction;
        private InputAction jumpAction;

        private Vector2 moveValue;
        private bool jumpTriggered;
        private bool jumpHeld;
        private bool readingInputs = true;

        public Vector2 MoveInput => moveValue;
        public bool JumpTriggered => jumpTriggered;
        public bool JumpHeld => jumpHeld;

        private void Awake()
        {
            moveAction = InputSystem.actions.FindAction("Move");
            jumpAction = InputSystem.actions.FindAction("Jump");
        }

        private void Update()
        {
            if (!readingInputs)
            {
                return;
            }

            if (moveAction != null) moveValue = moveAction.ReadValue<Vector2>();

            // https://docs.unity3d.com/Packages/com.unity.inputsystem@1.20/api/UnityEngine.InputSystem.InputAction.html#UnityEngine_InputSystem_InputAction_triggered
            if (jumpAction != null && jumpAction.triggered) jumpTriggered = true;
            if (jumpAction != null) jumpHeld = jumpAction.IsPressed();
        }

        public void ResetJumpTrigger() => jumpTriggered = false;

        public void StopReading()
        {
            readingInputs = false;
            moveValue = Vector2.zero;
            jumpTriggered = false;
            jumpHeld = false;
        }
    }
}