using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterSurvivors
{
    /// <summary>
    /// 虚拟摇杆 —— 用于触摸屏输入
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("组件引用")]
        [SerializeField] private RectTransform background;    // 摇杆底盘
        [SerializeField] private RectTransform handle;        // 摇杆手柄
        [SerializeField] private float maxRadius = 100f;      // 最大拖动半径

        public static Vector2 Value { get; private set; }     // 当前输入值 [-1, 1]
        public static bool IsActive { get; private set; }

        private Vector2 centerPosition;

        private void Start()
        {
            centerPosition = background.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsActive = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if(!IsActive) return;

            // 计算拖拽偏移
            Vector2 offset = eventData.position - centerPosition;

            // 限制在最大半径内
            if (offset.magnitude > maxRadius)
            {
                offset = offset.normalized * maxRadius;
            }

            // 更新手柄位置
            handle.position = centerPosition + offset;

            // 输出归一化值
            Value = offset / maxRadius;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsActive = false;
            Value = Vector2.zero;
            handle.position = centerPosition;
        }
    }
}