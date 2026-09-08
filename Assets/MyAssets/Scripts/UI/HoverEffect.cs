using UnityEngine;
using UnityEngine.EventSystems;

namespace MonsterSurvivors
{
    public class HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DoScale(originalScale * 1.1f, 0.15f, EasingType.BackOut);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DoScale(originalScale, 0.15f, EasingType.SineOut);
        }
    }
}
