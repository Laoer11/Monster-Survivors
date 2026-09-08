using UnityEngine;
using System.Collections;

namespace MonsterSurvivors
{
    /// <summary>
    /// 音频管理器 —— 单例，自动播放音乐和音效
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;

        [Header("数据库")]
        [SerializeField] private AudioDatabase database;

        [Header("音频源")]
        [SerializeField] private AudioSource musicSource;   // 播放 BGM
        [SerializeField] private AudioSource sfxSource;     // 播放音效（可扩展为多个）
        
        // ===== 音量属性（对接 SettingsSave） =====
        public static float musicVolume
        {
            get => instance.musicSource.volume;
            set => instance.musicSource.volume = Mathf.Clamp01(value);
        }

        public static float sfxVolume
        {
            get => instance.sfxSource.volume;
            set => instance.sfxSource.volume = Mathf.Clamp01(value);
        }

        // ===== 生命周期 =====

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // 如果没有拖 AudioSource，自动创建
            if (musicSource == null)
                musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
            sfxSource.loop = false;

            musicSource.volume = 0.8f;
            sfxSource.volume = 1f;


            GameController.RegisterAudioManager(this);
            Debug.Log("[AudioManager] 初始化完成");
        }

        private void Start()
        {
            // 从存档恢复音量
            var settings = GameController.SaveManager.GetSave<SettingsSave>("SettingsSave");
            musicSource.volume = settings.musicVolume;
            sfxSource.volume = settings.sfxVolume;
        }

        // ===== 音乐 API =====

        /// <summary>
        /// 播放音乐（自动切换，带淡入淡出）
        /// </summary>
        public static void PlayMusic(string name, float fadeDuration = 1.0f)
        {
            AudioClip clip = instance.database.GetMusic(name);
            if (clip == null) return;

            // 如果已经在播同一首，不重复播放
            if (instance.musicSource.clip == clip && instance.musicSource.isPlaying) return;

            instance.StartCoroutine(instance.SwitchMusicCoroutine(clip, fadeDuration));
        }

        private IEnumerator SwitchMusicCoroutine(AudioClip newClip, float fadeDuration)
        {
            //滚出
            if(musicSource.isPlaying) yield return FadeOut(musicSource, fadeDuration);

            // 切换曲目
            musicSource.clip = newClip;
            musicSource.Play();

            // 淡入
            yield return FadeIn(musicSource, fadeDuration);
        }
        
        /// <summary>
        /// 停止音乐
        /// </summary>
        public static void StopMusic(float fadeDuration = 0.3f)
        {
            instance.StartCoroutine(FadeOut(instance.musicSource, fadeDuration));
        }

        // ===== 音效 API =====

        /// <summary>
        /// 播放一个音效
        /// </summary>
        public static void PlaySFX(string name,float volumeScale = 1f)
        {
            AudioClip clip = instance.database.GetSFX(name);
            if(clip == null) return;

            instance.sfxSource.PlayOneShot(clip, volumeScale);
        }

        // ===== 淡入淡出辅助 =====
        private static IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;   // 不受 TimeScale 影响
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;   // 恢复音量（下次播放用）
        }

        private static IEnumerator FadeIn(AudioSource source, float duration)
        {
            float targetVolume = source.volume;
            source.volume = 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }

        // ===== 音量保存 =====

        /// <summary>
        /// 修改音量并持久化
        /// </summary>
        public static void SetMusicVolume(float value)
        {
            var settings = GameController.SaveManager.GetSave<SettingsSave>("SettingsSave");
            settings.musicVolume = Mathf.Clamp01(value);
            musicVolume = settings.musicVolume;
            // 立即保存到磁盘，确保下次启动生效
            GameController.SaveManager.SaveToDisk();
        }

        public static void SetSFXVolume(float value)
        {
            var settings = GameController.SaveManager.GetSave<SettingsSave>("SettingsSave");
            settings.sfxVolume = Mathf.Clamp01(value);
            sfxVolume = settings.sfxVolume;
            // 立即保存到磁盘，确保下次启动生效
            GameController.SaveManager.SaveToDisk();
        }

        private void OnDestroy()
        {
            if(instance == this) instance = null;
        }
    }
    
}
