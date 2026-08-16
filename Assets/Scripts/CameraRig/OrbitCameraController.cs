using UnityEngine;
using UnityEngine.InputSystem;

namespace Ascendra.CameraRig
{
    /// Mouse-driven orbit camera that follows a target at a fixed distance, Valheim-style.
    public class OrbitCameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.6f, 0f);

        [Header("Orbit")]
        [SerializeField] private float distance = 6f;
        [SerializeField] private float minDistance = 2f;
        [SerializeField] private float maxDistance = 12f;
        [SerializeField] private float mouseSensitivity = 0.15f;
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 70f;

        public Transform Target { get => target; set => target = value; }

        private InputAction lookAction;
        private InputAction zoomAction;
        private float yaw;
        private float pitch = 20f;

        private void Awake()
        {
            lookAction = new InputAction("Look", InputActionType.Value, "<Mouse>/delta");
            zoomAction = new InputAction("Zoom", InputActionType.Value, "<Mouse>/scroll/y");
        }

        private void OnEnable()
        {
            lookAction.Enable();
            zoomAction.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void OnDisable()
        {
            lookAction.Disable();
            zoomAction.Disable();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector2 lookDelta = lookAction.ReadValue<Vector2>();
            yaw += lookDelta.x * mouseSensitivity;
            pitch -= lookDelta.y * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            float scroll = zoomAction.ReadValue<float>();
            distance = Mathf.Clamp(distance - scroll * 0.01f, minDistance, maxDistance);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + targetOffset;
            Vector3 desiredPosition = pivot - rotation * Vector3.forward * distance;

            transform.SetPositionAndRotation(desiredPosition, rotation);
        }
    }
}
