using UnityEngine;
using UnityEngine.InputSystem;

namespace Ascendra.Player
{
    /// Third-person locomotion driven directly by the Input System (no .inputactions asset needed).
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 4.5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float turnSmoothTime = 0.1f;

        [Header("Jump & Gravity")]
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -18f;

        public Transform CameraPivot { get; set; }

        private CharacterController controller;
        private InputAction moveAction;
        private InputAction jumpAction;
        private InputAction sprintAction;

        private float verticalVelocity;
        private float turnSmoothVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();

            moveAction = new InputAction("Move", InputActionType.Value, expectedControlType: "Vector2");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");

            jumpAction = new InputAction("Jump", InputActionType.Button, "<Keyboard>/space");
            sprintAction = new InputAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");
        }

        private void OnEnable()
        {
            moveAction.Enable();
            jumpAction.Enable();
            sprintAction.Enable();
        }

        private void OnDisable()
        {
            moveAction.Disable();
            jumpAction.Disable();
            sprintAction.Disable();
        }

        private void Update()
        {
            Vector2 input = moveAction.ReadValue<Vector2>();
            Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

            bool isGrounded = controller.isGrounded;
            if (isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f; // small downward force keeps the controller grounded
            }

            Vector3 horizontalMotion = Vector3.zero;
            if (moveDirection.magnitude >= 0.1f && CameraPivot != null)
            {
                float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg + CameraPivot.eulerAngles.y;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);

                float speed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;
                horizontalMotion = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward * speed;
            }

            if (jumpAction.WasPressedThisFrame() && isGrounded)
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            verticalVelocity += gravity * Time.deltaTime;

            Vector3 motion = horizontalMotion;
            motion.y = verticalVelocity;
            controller.Move(motion * Time.deltaTime);
        }
    }
}
