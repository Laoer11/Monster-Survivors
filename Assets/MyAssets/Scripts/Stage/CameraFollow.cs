using UnityEngine;

namespace MonsterSurvivors
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("跟随阻尼")]
        [SerializeField] private Transform target;
        [SerializeField] private float followSmoothTime = 0.2f;   // 阻尼时间，越小跟得越紧

        private Vector3 followVelocity;

        private void Start()
        {
            if (target == null && PlayerBehavior.Instance != null)
                target = PlayerBehavior.Instance.transform;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desired = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(
                transform.position, desired, ref followVelocity, followSmoothTime);
        }
    }
}
