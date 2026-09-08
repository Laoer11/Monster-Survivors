using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace MonsterSurvivors
{
    public class SettingsPopup : MonoBehaviour
    {
        [Header("弹窗")]
        [SerializeField] private GameObject popupRoot;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform windowTransform;

        [Header("音乐")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private TMP_Text musicLabel;

        [Header("音效")]
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private TMP_Text sfxLabel;

        [Header("按钮")]
        [SerializeField] private Button btnCloseX;           // ✅ 右上角 X 叉叉按钮 —— 永远只关弹窗
        [SerializeField] private Button btnBottomAction;     // ✅ 底部大按钮 —— 智能：MainMenu=关闭 / Game=返回主菜单
        [SerializeField] private TMP_Text btnBottomLabel;    // 底部按钮的文字组件，用于动态切换"关闭"/"返回主菜单"
        [SerializeField] private Button btnDeleteSave;       // 删除存档按钮

        private bool isOpen;
        private bool isLoading;   // 加载标记，防止初始化时Slider赋值触发回调

        /// <summary>
        /// 弹窗是否打开（供外部检测，如GameUIController）
        /// </summary>
        public bool IsOpen => isOpen;

        private void Start()
        {
            isLoading = true;

            // 从存档恢复滑块值（SetValueWithoutNotify 防止赋值触发回调）
            var settings = GameController.SaveManager.GetSave<SettingsSave>("SettingsSave");
            musicSlider.SetValueWithoutNotify(settings.musicVolume);
            sfxSlider.SetValueWithoutNotify(settings.sfxVolume);
            UpdateMusicLabel(settings.musicVolume);
            UpdateSFXLabel(settings.sfxVolume);

            // 绑定事件
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);

            // 【分工明确】
            // 1) 右上角叉叉 → 永远执行关闭弹窗（不做场景判断，不返回主菜单）
            if (btnCloseX != null)
                btnCloseX.onClick.AddListener(OnXButtonClicked);
            else
                Debug.LogError("[SettingsPopup] 请把右上角的 X 叉叉按钮拖到 btnCloseX 字段！");

            // 2) 底部大按钮 → 智能响应（MainMenu=关闭 / Game=返回主菜单）
            if (btnBottomAction != null)
                btnBottomAction.onClick.AddListener(OnBottomButtonClicked);
            else
                Debug.LogError("[SettingsPopup] 请把底部大按钮拖到 btnBottomAction 字段！");

            btnDeleteSave.onClick.AddListener(OnDeleteSave);

            // 根据当前场景设置底部按钮的文字和用途
            SetupSmartButton();

            isLoading = false;

            // 初始状态：关闭
            popupRoot.SetActive(false);
        }

        // 判断当前是否处于 Game 场景（复用方法）
        private bool IsInGameScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name.Contains("Game"))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 根据当前场景自动配置底部按钮的文字
        /// - Game场景：按钮变成"返回主菜单"
        /// - 其他场景（MainMenu/Lobby等）：按钮保持"关闭"
        /// </summary>
        private void SetupSmartButton()
        {
            bool inGameScene = IsInGameScene();

            // 尝试获取底部按钮上的文字组件
            TMP_Text label = btnBottomLabel;
            if (label == null && btnBottomAction != null)
            {
                label = btnBottomAction.GetComponentInChildren<TMP_Text>();
            }

            if (inGameScene)
            {
                if (label != null) label.text = "返回主菜单";
                Debug.Log("[SettingsPopup] 检测到 Game 场景 → 底部按钮文字改为【返回主菜单】，右上角 X 保持【关闭弹窗】");
            }
            else
            {
                if (label != null) label.text = "关闭";
                Debug.Log("[SettingsPopup] 非 Game 场景 → 底部按钮文字为【关闭】，右上角 X 保持【关闭弹窗】");
            }
        }

        /// <summary>
        /// 【右上角 X 叉叉】—— 永远只关闭弹窗，不做任何场景跳转
        /// </summary>
        private void OnXButtonClicked()
        {
            AudioManager.PlaySFX("Click");
            Debug.Log("[SettingsPopup] 右上角 X 被点击 → 仅关闭弹窗");
            Hide();
        }

        /// <summary>
        /// 【底部大按钮】—— 智能响应：
        /// - MainMenu/Lobby 场景：只关弹窗
        /// - Game 场景：关闭弹窗 + 返回主菜单
        /// </summary>
        private void OnBottomButtonClicked()
        {
            AudioManager.PlaySFX("Click");

            if (IsInGameScene())
            {
                Debug.Log("[SettingsPopup] 底部按钮被点击 → Game 场景：关闭弹窗 + 返回主菜单");
                Hide();
                GameController.LoadMainMenu();
            }
            else
            {
                Debug.Log("[SettingsPopup] 底部按钮被点击 → 非 Game 场景：仅关闭弹窗");
                Hide();
            }
        }

        private void Update()
        {
            // Esc 关闭
            if (isOpen && UnityEngine.InputSystem.Keyboard.current != null
                && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Hide();
            }
        }

        public void Show()
        {
            if (isOpen) return;
            isOpen = true;

            popupRoot.SetActive(true);

            canvasGroup.alpha = 0f;
            windowTransform.localScale = Vector3.zero;

            // 遮罩淡入 + 弹窗弹入，同时进行
            canvasGroup.DoFade(1f, 0.25f);
            windowTransform.DoPopIn(0.35f);

            //GameController.InputManager.DisableGameplayInput();   // 弹出时禁用游戏操作（后面实现）
        }

        public void Hide()
        {
            if (!isOpen) return;
            isOpen = false;

            //GameController.InputManager.EnableGameplayInput();    // 关闭时恢复

            windowTransform.DoPopOut(0.15f);
            canvasGroup.DoFade(0f, 0.15f)
                .SetOnFinish(() => popupRoot.SetActive(false));
        }

        private void OnMusicChanged(float value)
        {
            if (isLoading) return;  // 初始化加载中不响应，避免覆盖存档
            AudioManager.SetMusicVolume(value);
            UpdateMusicLabel(value);
        }

        private void OnSFXChanged(float value)
        {
            if (isLoading) return;  // 初始化加载中不响应，避免覆盖存档
            AudioManager.SetSFXVolume(value);
            UpdateSFXLabel(value);
        }

        private void UpdateMusicLabel(float value)
        {
            musicLabel.text = $"音乐: {Mathf.RoundToInt(value * 100)}%";
        }

        private void UpdateSFXLabel(float value)
        {
            sfxLabel.text = $"音效: {Mathf.RoundToInt(value * 100)}%";
        }

        private void OnDeleteSave()
        {
            GameController.SaveManager.DeleteAllSaves();
            Debug.Log("[Settings] 存档已清空");
            // 点 "返回主菜单" 按钮 → 关闭弹窗 + 回主菜单
            AudioManager.PlaySFX("Click");
            Hide();
            GameController.LoadMainMenu();
        }
    }
}
