using System;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 音频数据库 —— 存储所有音频资源
    /// </summary>
    [CreateAssetMenu(fileName = "AudioDatabase", menuName = "MonsterSurvivors/Audio Database")]
    public class AudioDatabase : ScriptableObject
    {
        [Header("背景音乐")]
        public List<MusicEntry> musicList = new List<MusicEntry>();

        [Header("音效")]
        public List<SFXEntry> sfxList = new List<SFXEntry>();

        /// <summary>
        /// 根据名字获取音乐
        /// </summary>
        public AudioClip GetMusic(string name)
        {
            foreach (var entry in musicList)
            {
                if (entry.name == name) return entry.clip;    
            }
            Debug.LogWarning($"[AudioDatabase] 找不到音乐: {name}");
            return null;
        }

        /// <summary>
        /// 根据名字获取音效
        /// </summary>
        public AudioClip GetSFX(string name)
        {
            foreach (var entry in sfxList)
            {
                if(entry.name == name) return entry.clip;
            }
            Debug.LogWarning($"[AudioDatabase] 找不到音效: {name}");
            return null;
        }
    }

    /// <summary>
    /// 音乐条目
    /// </summary>
    [Serializable]
    public class MusicEntry
    {
        public string name;     // 例如 "MainMenu"
        public AudioClip clip;  // 音频文件
        [Range(0f,1f)]
        public float volume = 1f;//默认音量
    }

    /// <summary>
    /// 音效条目
    /// </summary>
    [Serializable]
    public class SFXEntry
    {
        public string name;     // 例如 "Hit"
        public AudioClip clip;  // 音频文件
        [Range(0f,1f)]
        public float volume = 1f;//默认音量
    }
}

