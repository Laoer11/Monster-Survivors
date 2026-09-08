using System;
using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 封装一个动画协程，支持链式调用 SetOnFinish / Stop
    /// </summary>
    public class EasingCoroutine
    {
        public Coroutine coroutineRef;       // Unity 协程引用
        private IEnumerator userCoroutine;   // 用户定义的动画逻辑
        private Action onFinishCallback;     // 动画结束回调
        private bool stopped = false;

        public EasingCoroutine(IEnumerator coroutine)
        {
            userCoroutine = coroutine;
        }

        /// <summary>
        /// 实际执行的协程：先跑用户动画，再触发 onFinish
        /// </summary>
        public IEnumerator WrappedCoroutine()
        {
            yield return userCoroutine;
            if (!stopped)
            {
                onFinishCallback?.Invoke();
            }
        }

        /// <summary>
        /// 设置动画结束时的回调（链式调用）
        /// 用法：transform.DoMove(xxx).SetOnFinish(() => Debug.Log("Done!"))
        /// </summary>
        public EasingCoroutine SetOnFinish(Action callback)
        {
            onFinishCallback = callback;
            return this;
        }

        /// <summary>
        /// 停止动画
        /// </summary>
        public void Stop()
        {
            stopped = true;
            // 【修复】应用退出/场景清理阶段Instance可能返回null（为了防止在OnDestroy中new GameObject）
            // 此时直接跳过即可，Unity会在场景卸载时自动终止所有协程
            var mgr = EasingManager.Instance;
            if (mgr != null)
            {
                mgr.StopAnimation(this);
            }
        }
    }
}
