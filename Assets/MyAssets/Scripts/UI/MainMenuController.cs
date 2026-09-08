using UnityEngine;
using UnityEngine.UI;

namespace MonsterSurvivors
{
    /// <summary>
    /// 主菜单场景控制器 —— 管理 UI 动画和按钮事件
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform titleTransform;
        [SerializeField] private Button btnStart;
        [SerializeField] private Button btnSettings;
        [SerializeField] private SettingsPopup settingsPopup;

        // 回调给外部
        public static System.Action onCharacterClicked;

        private System.Collections.Generic.List<EasingCoroutine> runningAnimations 
            = new System.Collections.Generic.List<EasingCoroutine>();


        private void Start()
        {
            btnStart.onClick.AddListener(OnStartClicked);
            btnSettings.onClick.AddListener(() => settingsPopup.Show());

            AudioManager.PlayMusic("MainMenu", 0.5f);

            PlayEnterAnimation();
        }

        private void OnDestroy()
        {
            foreach (var anim in runningAnimations)
            {
                anim?.Stop();
            }
            runningAnimations.Clear();
        }


        private void PlayEnterAnimation()
        {
            // 检查 EasingManager 是否可用
            var mgr = EasingManager.Instance;
            if (mgr == null)
            {
                Debug.LogError("[MainMenuController] EasingManager.Instance 为空，跳过进场动画（但保证界面可见）");
                // 防御：不播动画也要强制显示UI，否则界面全黑按钮看不见
                canvasGroup.alpha = 1f;
                titleTransform.anchoredPosition = new Vector2(0, 400);
                btnStart.transform.localScale = Vector3.one;
                btnSettings.transform.localScale = Vector3.one;
                return;
            }

            //整体淡入
            canvasGroup.alpha = 0;
            runningAnimations.Add(mgr.RunAnimation(FadeInCoroutine()));

            // Logo从上方滑入
            titleTransform.anchoredPosition = new Vector2(0, 500);
            runningAnimations.Add(
                titleTransform.DoMove(titleTransform.position.SetY(titleTransform.position.y - 100),
                    0.6f, EasingType.BackOut)
            );

            // 按钮延迟弹出
            btnStart.transform.localScale = Vector3.zero;
            btnSettings.transform.localScale = Vector3.zero;

            runningAnimations.Add(
                mgr.DoAfter(0.4f, () =>
                {
                    if (btnStart != null) btnStart.transform.DoPopIn(0.35f);
                })
            );
            runningAnimations.Add(
                mgr.DoAfter(0.55f, () =>
                {
                    if (btnSettings != null) btnSettings.transform.DoPopIn(0.35f);
                })
            );
        }

        private System.Collections.IEnumerator FadeInCoroutine()
        {
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup == null) yield break;

                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        private void OnStartClicked()
        {
            AudioManager.PlaySFX("Click");
            GameController.LoadLobby();
        }

    }
}