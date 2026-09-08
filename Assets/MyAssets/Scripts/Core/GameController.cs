using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


namespace MonsterSurvivors
{
    /// <summary>
    /// 全局游戏控制器 —— 单例，跨场景不销毁
    /// 负责场景切换、管理全局服务
    /// </summary>
    public class GameController : MonoBehaviour
    {     
        //单例模式
        private static GameController instance;

        //场景名称配置
        [Header("场景名称")]
        [SerializeField]private string mainMenuSceneName = "MainMenu";
        [SerializeField]private string gameSceneName = "Game";
        [SerializeField]private string loadingSceneName = "Loading";
        [SerializeField] private string lobbySceneName = "Lobby";
        [SerializeField] private string stageSelectSceneName = "StageSelect";



        //全局服务引用
        public static SaveManager SaveManager { get; private set; }     //存档管理器
        public static InputManager InputManager { get; private set; }   //输入管理器
        public static AudioManager AudioManager { get; private set; }   //音频管理器
        public static UpgradesManager UpgradesManager { get; private set; }

        public static bool FirstTimeLoaded {get;private set;}

        private void Start()
{
        // Boot 启动后自动加载主菜单
        if (!IsSceneLoaded(mainMenuSceneName))
        {
            StartCoroutine(MainMenuLoadingCoroutine());
        }
}

        //生命周期
        private void Awake()
        {
            // 单例检测：如果已存在实例，销毁当前对象
            if(instance != null)
            {
                Destroy(gameObject);
                FirstTimeLoaded = false;
                return;
            }
            instance = this;
            //设置为不销毁
            DontDestroyOnLoad(gameObject);
            FirstTimeLoaded = true;

            //设置目标频率
            Application.targetFrameRate = 60;
            Debug.Log("[GameController] 初始化完成");
        }

        // ========== 服务注册 ==========

        public static void RegisterSaveManager(SaveManager saveManager)
        {
            SaveManager = saveManager;
            Debug.Log("[GameController] SaveManager 已注册");
        }

        public static void RegisterInputManager(InputManager inputManager)
        {
            InputManager = inputManager;
            Debug.Log("[GameController] InputManager 已注册");
        }

        public static void RegisterAudioManager(AudioManager audioManager)
        {
            AudioManager = audioManager;
            Debug.Log("[GameController] AudioManager 已注册");
        }

        public static void RegisterUpgradesManager(UpgradesManager manager)
        {
            UpgradesManager = manager;
            Debug.Log("[GameController] UpgradesManager 已注册");
        }


        // ========== 场景切换 ==========
        public static void LoadStageSelect()
        {
            if (instance == null) return;
            instance.StartCoroutine(instance.LoadStageSelectCoroutine());
        }

        private IEnumerator LoadStageSelectCoroutine()
        {
            if (IsSceneLoaded(lobbySceneName))
                yield return SceneManager.UnloadSceneAsync(lobbySceneName);

            if (!IsSceneLoaded(stageSelectSceneName))
                yield return SceneManager.LoadSceneAsync(stageSelectSceneName, LoadSceneMode.Additive);
        }


        /// <summary>
        /// 从主菜单加载游戏关卡
        /// </summary>
        public static void LoadStage(string stageSceneName = null)
        {
            if(instance == null) return;
            instance.StartCoroutine(instance.StageLoadingCoroutine(stageSceneName));
        }

        /// <summary>
        /// 从游戏关卡返回主菜单
        /// </summary>
        public static void LoadMainMenu()
        {
            if (instance == null) return;
            // 【防御】切场景前强制恢复时间缩放，防止 Game 里用 timeScale=0 暂停后回到主菜单导致 UI/动画卡死
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;   // Unity 默认 fixedDeltaTime = 0.02 * timeScale，顺便还原基准值
            instance.StartCoroutine(instance.MainMenuLoadingCoroutine());
        }

        private IEnumerator StageLoadingCoroutine(string stageSceneName)
        {
            string targetScene = stageSceneName ?? gameSceneName;

            // 进 Game 前，必须把前面的场景全卸载
            if (IsSceneLoaded(stageSelectSceneName))
                yield return SceneManager.UnloadSceneAsync(stageSelectSceneName);

            if (IsSceneLoaded(lobbySceneName))
                yield return SceneManager.UnloadSceneAsync(lobbySceneName);

            if (!IsSceneLoaded(targetScene))
                yield return SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Additive);
        }


        private IEnumerator MainMenuLoadingCoroutine()
        {
            Debug.Log("[GameController] 开始加载主菜单...");

            // 只有游戏关卡在运行时才卸载它
            if (IsSceneLoaded(gameSceneName))
                yield return SceneManager.UnloadSceneAsync(gameSceneName);

            if (!IsSceneLoaded(mainMenuSceneName))
                yield return SceneManager.LoadSceneAsync(mainMenuSceneName, LoadSceneMode.Additive);

            Debug.Log("[GameController] 主菜单加载完成");
        }

        public static void LoadLobby()
        {
            if (instance == null) return;
            instance.StartCoroutine(instance.LobbyLoadingCoroutine());
        }

        private IEnumerator LobbyLoadingCoroutine()
        {
            Debug.Log("[GameController] 开始加载 Lobby...");
            if (IsSceneLoaded(gameSceneName))
                yield return SceneManager.UnloadSceneAsync(gameSceneName);

            if (IsSceneLoaded(mainMenuSceneName))
                yield return SceneManager.UnloadSceneAsync(mainMenuSceneName);

            if (IsSceneLoaded(stageSelectSceneName))                              
                yield return SceneManager.UnloadSceneAsync(stageSelectSceneName); 


            if (!IsSceneLoaded(lobbySceneName))
                yield return SceneManager.LoadSceneAsync(lobbySceneName, LoadSceneMode.Additive);

            Debug.Log("[GameController] Lobby 加载完成");

        }


        /// <summary>
        /// 检查场景是否已被加载
        /// </summary>
        private static bool IsSceneLoaded(string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneName)
                    return true;
            }
            return false;
        }


    }

}