using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace MonsterSurvivors
{
    /// <summary>
    /// 缓动动画管理器 —— 提供协程宿主，让扩展方法能"跑起来"
    /// 挂在 Boot 场景的持久化对象上
    /// </summary>
    public class EasingManager : MonoBehaviour
    {
        private static EasingManager instance;
        private static bool applicationIsQuitting = false;

        // 追踪所有运行中的协程，用于清理（避免残留回调空引用）
        private List<EasingCoroutine> activeCoroutines = new List<EasingCoroutine>();

        public static EasingManager Instance
        {
            get
            {
                // 【修复1】应用退出/场景清理阶段，禁止动态创建单例，否则触发Unity警告
                if (applicationIsQuitting) return null;

                if (instance == null)
                {
                    // 如果没有，创建一个（兜底）
                    GameObject go = new GameObject("[EasingManager]");
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<EasingManager>();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
            applicationIsQuitting = false;
        }

        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
            Cleanup();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                Cleanup();
                instance = null;
            }
        }

        /// <summary>
        /// 停止所有运行中的协程并清理引用
        /// </summary>
        private void Cleanup()
        {
            StopAllCoroutines();
            foreach (var cor in activeCoroutines)
            {
                cor?.Stop();
            }
            activeCoroutines.Clear();
        }

        ///<summary>
        /// 执行一个动画协程
        ///</summary>
        public EasingCoroutine RunAnimation(IEnumerator coroutine)
        {
            EasingCoroutine wrapper = new EasingCoroutine(coroutine);
            wrapper.coroutineRef = StartCoroutine(TrackCoroutine(wrapper));
            activeCoroutines.Add(wrapper);
            return wrapper;
        }

        /// <summary>
        /// 包装协程：跑完后从追踪列表中移除
        /// </summary>
        private IEnumerator TrackCoroutine(EasingCoroutine wrapper)
        {
            yield return wrapper.WrappedCoroutine();
            activeCoroutines.Remove(wrapper);
        }

        /// <summary>
        /// 停止一个动画
        /// </summary>
        public void StopAnimation(EasingCoroutine coroutine)
        {
            if (coroutine != null && coroutine.coroutineRef != null)
            {
                StopCoroutine(coroutine.coroutineRef);
                activeCoroutines.Remove(coroutine);
            }
        }

        /// <summary>
        /// 延迟执行
        /// </summary>
        public EasingCoroutine DoAfter(float delay, Action callback)
        {
            return RunAnimation(DoAfterCoroutine(delay, callback));
        }

        private IEnumerator DoAfterCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }

    }
}

