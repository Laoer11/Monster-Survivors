using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 命中特效 —— 生成时放大 + 淡出，播完自动隐藏
    /// </summary>
    public class HitVFX : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void OnEnable()
        {
            transform.localScale = Vector3.one * 0.3f;
            spriteRenderer.SetAlpha(1f);

            // 放大
            transform.DoScale(Vector3.one * 1.2f, 0.2f, EasingType.BackOut);

            // 淡出消失
            spriteRenderer.DoAlpha(0f, 0.25f, EasingType.QuadOut).SetOnFinish(
                () => gameObject.SetActive(false)   );
        }
    }
}