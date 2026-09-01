using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Ascendra.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class ThirdPersonStickController : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 12f;
        [SerializeField] private float gravity = -20f;

        private CharacterController characterController;
        private float verticalVelocity;

        public void Initialize(Transform followCamera)
        {
            cameraTransform = followCamera;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector2 input = ReadMoveInput();
            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            Vector3 movement = (forward * input.y) + (right * input.x);
            movement = Vector3.ClampMagnitude(movement, 1f);

            if (movement.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            verticalVelocity = characterController.isGrounded
                ? -1f
                : verticalVelocity + (gravity * Time.deltaTime);

            Vector3 velocity = (movement * moveSpeed) + (Vector3.up * verticalVelocity);
            characterController.Move(velocity * Time.deltaTime);
        }

        private static Vector2 ReadMoveInput()
        {
#if ENABLE_INPUT_SYSTEM
            Keyboard keyboard = Keyboard.current;
            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float horizontal = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1f : 0f);
            float vertical = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1f : 0f)
                - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1f : 0f);
            return new Vector2(horizontal, vertical);
#else
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif
        }
    }
}
