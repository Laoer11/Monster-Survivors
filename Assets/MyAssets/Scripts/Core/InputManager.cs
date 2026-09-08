using UnityEngine;
using UnityEngine.InputSystem;

namespace MonsterSurvivors
{
    /// <summary>
    /// 输入管理器 —— 单例，自动检测输入类型切换
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;

        [Header("Input Actions 配置")]
        [SerializeField] private InputActionAsset inputActions;

        private InputAction moveAction;
        private InputAction pauseAction;

        // ===== 公开属性 =====
        public static Vector2 Movement{get; private set;}
        public static InputType ActiveInputType { get; private set; }
        public static InputManager Instance => instance;

        public static bool PausePressed{get;private set;}

        // ===== 生命周期 =====
        private void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            // 从配置文件中取出 "Gameplay/Move" 这个 Action
            var gameplayMap = inputActions.FindActionMap("Gameplay");
            moveAction = gameplayMap.FindAction("Move");

            GameController.RegisterInputManager(this);
            Debug.Log("[InputManager] 初始化完成");

            // 从配置文件中取出 "Gameplay/Move" 这个 Action
            pauseAction = gameplayMap.FindAction("Pause");

        }
        private void OnEnable() => inputActions?.Enable();
        private void OnDisable() => inputActions?.Disable();

        private void Update()
        {
            DetectInputTypeSwitch();
            Movement = moveAction.ReadValue<Vector2>();

            PausePressed = pauseAction.WasPressedThisFrame();
        }

        // ===== 输入类型检测 =====
        private void DetectInputTypeSwitch()
        {
            // 检测键鼠
            if(Keyboard.current != null && Keyboard.current.wasUpdatedThisFrame
                && !Keyboard.current.CheckStateIsAtDefaultIgnoringNoise())
            {
                SwitchTo(InputType.Keyboard);
                return;
            }
            //检测触屏
            if(Touchscreen.current != null && Touchscreen.current.wasUpdatedThisFrame)
            {
                SwitchTo(InputType.Touchscreen);
                return;
            }
        }

        private void SwitchTo(InputType newType)
        {
            if(ActiveInputType == newType) return;
            var oldType = ActiveInputType;
            ActiveInputType = newType;
            Debug.Log($"[InputManager] 输入切换: {oldType} → {newType}");
        }

        private void OnDestroy()
        {
            if (instance == this) instance = null;
        }

    }
    /// <summary>
    /// 输入设备类型
    /// </summary>
    public enum InputType
    {
        Keyboard,
        Touchscreen,
    }
}