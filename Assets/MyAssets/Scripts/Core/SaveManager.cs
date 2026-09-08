using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 通用存档管理器
    /// 将所有 ISave 对象打包成 JSON 存入 PlayerPrefs
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        // ========== 单例 ==========
        private static SaveManager instance;

        // ========== 存档数据容器 ==========
        // 用字典存储所有存档对象，key 是存档的唯一名称
        private Dictionary<string, ISave> saves = new Dictionary<string, ISave>();

        // ========== 生命周期 ==========

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            // 尝试从磁盘加载存档
            Load();

            // 向 GameController 注册自己
            GameController.RegisterSaveManager(this);

            Debug.Log("[SaveManager] 初始化完成");
        }

        // ========== 核心 API ==========

        /// <summary>
        /// 获取一个存档。如果不存在则创建新的。
        /// 这是你最常用的方法！
        /// </summary>
        /// <typeparam name="T">存档类型，必须实现 ISave 且有无参构造函数</typeparam>
        /// <param name="key">唯一标识（建议用存档类型的名字）</param>
        public T GetSave<T>(string key) where T : ISave, new()
        {
            // 如果字典里已经有了，直接返回
            if (saves.TryGetValue(key, out ISave existing))
            {
                return (T)existing;
            }

            // 否则创建一个全新的存档对象
            T newSave = new T();
            newSave.Init();
            saves[key] = newSave;

            Debug.Log($"[SaveManager] 创建新存档: {key}");

            return newSave;
        }

        /// <summary>
        /// 把当前所有存档写入磁盘
        /// </summary>
        public void SaveToDisk()
        {
            // 1. 通知每个存档：马上要保存了
            foreach (var kvp in saves)
            {
                kvp.Value.Flush();
            }

            // 2. 序列化为 JSON
            string json = SerializeAll();
            // 3. 写入 PlayerPrefs
            PlayerPrefs.SetString("monster_survivors_save", json);
            PlayerPrefs.Save();

            Debug.Log("[SaveManager] 存档已保存到磁盘");
        }

        /// <summary>
        /// 清空所有存档（删号重来）
        /// </summary>
        public void DeleteAllSaves()
        {
            saves.Clear();
            PlayerPrefs.DeleteKey("monster_survivors_save");
            Debug.Log("[SaveManager] 所有存档已清空");
        }

        // ========== 内部实现 ==========

        /// <summary>
        /// 从磁盘加载存档
        /// </summary>
        private void Load()
        {
            string json = PlayerPrefs.GetString("monster_survivors_save", "");

            if (string.IsNullOrEmpty(json))
            {
                Debug.Log("[SaveManager] 没有找到存档，将使用默认值");
                return;
            }

            try
            {
                DeserializeAll(json);
                Debug.Log("[SaveManager] 存档加载成功");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 存档损坏，已重置: {e.Message}");
                saves.Clear();
            }
        }

        /// <summary>
        /// 把所有存档对象打包成一个 JSON
        /// </summary>
        private string SerializeAll()
        {
            // 创建包装对象，方便 JSON 序列化
            SaveWrapper wrapper = new SaveWrapper();
            wrapper.saveEntries = new List<SaveEntry>();

            foreach (var kvp in saves)
            {
                SaveEntry entry = new SaveEntry
                {
                    key = kvp.Key,
                    typeName = kvp.Value.GetType().AssemblyQualifiedName,
                    jsonData = JsonUtility.ToJson(kvp.Value)   // 每种存档各自转 JSON
                };
                wrapper.saveEntries.Add(entry);
            }

            return JsonUtility.ToJson(wrapper);
        }

        /// <summary>
        /// 从 JSON 还原所有存档对象
        /// </summary>
        private void DeserializeAll(string json)
        {
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(json);

            if (wrapper == null || wrapper.saveEntries == null) return;

            foreach (var entry in wrapper.saveEntries)
            {
                // 根据类型名还原存档对象
                Type type = Type.GetType(entry.typeName);
                if (type == null)
                {
                    Debug.LogWarning($"[SaveManager] 找不到类型: {entry.typeName}，跳过");
                    continue;
                }

                ISave saved = (ISave)JsonUtility.FromJson(entry.jsonData, type);
                if (saved != null)
                {
                    saves[entry.key] = saved;
                }
            }
        }

        private void OnDestroy()
        {
            // 对象销毁前自动保存
            SaveToDisk();
        }

        private void OnApplicationPause(bool pause)
        {
            // 手机切换到后台时自动保存
            if (pause) SaveToDisk();
        }
    }

    // ========== JSON 包装结构 ==========

    /// <summary>
    /// 存档文件的顶层结构，包含所有存档条目
    /// </summary>
    [Serializable]
    public class SaveWrapper
    {
        public List<SaveEntry> saveEntries;
    }

    /// <summary>
    /// 单个存档条目：key（名称）+ typeName（类型）+ jsonData（内容）
    /// </summary>
    [Serializable]
    public class SaveEntry
    {
        public string key;
        public string typeName;
        public string jsonData;
    }
}