using UnityEngine;
using UnityEngine.UI;

namespace MonsterSurvivors
{
    public class LobbyController : MonoBehaviour
    {
        [Header("动画")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform titleTransform;
        [SerializeField] private Button btnCharacters;
        [SerializeField] private Button btnStageSelect;
        [SerializeField] private Button btnBack;

        private void Start()
        {
            btnCharacters.onClick.AddListener(OnCharactersClicked);
            btnStageSelect.onClick.AddListener(OnStageSelectClicked);
            btnBack.onClick.AddListener(OnBackClicked);

            PlayEnterAnimation();
        }

        private void PlayEnterAnimation()
        {
            var mgr = EasingManager.Instance;
            if (mgr == null)
            {
                Debug.LogWarning("[LobbyController] EasingManager 不可用，跳过进场动画");
                // 即使不播动画，也要把UI显示出来，否则界面全黑/看不见按钮
                canvasGroup.alpha = 1f;
                return;
            }

            canvasGroup.alpha = 0f;
            mgr.RunAnimation(FadeInCoroutine());

            titleTransform.anchoredPosition = new Vector2(0, 250);
            titleTransform.DoMove(titleTransform.position.SetY(titleTransform.position.y - 120),
                0.5f, EasingType.BackOut);

            btnCharacters.transform.localScale = Vector3.zero;
            btnStageSelect.transform.localScale = Vector3.zero;
            btnBack.transform.localScale = Vector3.zero;

            mgr.DoAfter(0.3f, () => btnCharacters.transform.DoPopIn(0.3f));
            mgr.DoAfter(0.45f, () => btnStageSelect.transform.DoPopIn(0.3f));
            mgr.DoAfter(0.6f, () => btnBack.transform.DoPopIn(0.3f));
        }

        private System.Collections.IEnumerator FadeInCoroutine()
        {
            float elapsed = 0f;
            float duration = 0.4f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        private void OnCharactersClicked()
        {
            AudioManager.PlaySFX("Click");
            Debug.Log("[Lobby] 角色养成 - 后续课程实现");
            // 后续这里加载角色养成场景
        }

        private void OnStageSelectClicked()
        {
            AudioManager.PlaySFX("Click");
            Debug.Log("[Lobby] 关卡选择 - 进入游戏");
            GameController.LoadStageSelect();
        }

        private void OnBackClicked()
        {
            AudioManager.PlaySFX("Click");
            GameController.LoadMainMenu();
        }
    }
}
