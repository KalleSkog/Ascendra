using UnityEngine;

namespace Ascendra.CameraRig
{
    public sealed class ThirdPersonCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
        [SerializeField] private float followSharpness = 12f;

        public void Initialize(Transform followTarget)
        {
            target = followTarget;
            SnapToTarget();
        }

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            Vector3 desiredPosition = target.TransformPoint(offset);
            float blend = 1f - Mathf.Exp(-followSharpness * Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, blend);
            transform.LookAt(target.position + (Vector3.up * 1.5f));
        }

        private void SnapToTarget()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.TransformPoint(offset);
            transform.LookAt(target.position + (Vector3.up * 1.5f));
        }
    }
}
