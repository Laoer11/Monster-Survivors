using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    public class FireballAbilityBehavior : MonoBehaviour, IAbilityBehavior
    {
        [Header("火球预制体")]
        [SerializeField] private FireballProjectile fireballPrefab;

        [Header("每级参数")]
        [SerializeField] private float[] cooldowns = { 3f, 2.5f, 2f, 1.5f, 1f};
        [SerializeField] private float[] damages = { 10f, 15f, 22f, 33f, 50f };
        [SerializeField] private float[] speeds = { 6f, 7f, 8f, 9f, 10f };
        [SerializeField] private float lifetime = 3f;

        private int level = 0;
        private Coroutine fireCoroutine;
        private float currentCooldown;
        private Pool<FireballProjectile> pool;

        public void Init(int abilityLevel)
        {
            level = abilityLevel;
            currentCooldown = cooldowns[level];
            PlayerBehavior.CooldownMultiplierChanged += OnCooldownChanged;

            // 初始化火球对象池（预创建 5 个，不跨场景持久化）
            pool = new Pool<FireballProjectile>(fireballPrefab, 5, "FireballPool", false);

            fireCoroutine = StartCoroutine(FireLoop());
        }

        public void Upgrade(int newLevel)
        {
            level = newLevel;
            currentCooldown = cooldowns[level];
            // 重启协程以应用新冷却时间
            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireLoop());
        }

        private void OnCooldownChanged(float newMultiplier)
        {
            currentCooldown = cooldowns[level] * newMultiplier;
            Debug.Log($"[Fireball] 冷却倍率变化: {newMultiplier}, 实际冷却: {currentCooldown}");
        }

        private void OnDestroy()
        {
            PlayerBehavior.CooldownMultiplierChanged -= OnCooldownChanged;
        }


        private IEnumerator FireLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldowns[level]);
                
                // 找最近敌人
                EnemyBehavior target = FindNearestEnemy();
                if (target == null) continue;

                // 从池中取出火球
                FireballProjectile fb = GetFireball();
                fb.transform.position = PlayerBehavior.Instance.transform.position;

                Vector2 dir = (target.transform.position - fb.transform.position).normalized;
                fb.Launch(dir, speeds[level], damages[level], lifetime);
            }
        }

        private EnemyBehavior FindNearestEnemy()
        {
            EnemyBehavior[] all = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None);
            EnemyBehavior nearest = null;
            float minDist = float.MaxValue;

            Vector2 playerPos = PlayerBehavior.Instance.transform.position;

            foreach(var e in all)
            {
                float dist = Vector2.SqrMagnitude((Vector2)e.transform.position - playerPos);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = e;
                }
            }

            return nearest;
        }

        private FireballProjectile GetFireball()
        {
            FireballProjectile fb = pool.Get();
            fb.SetReturnCallback(pool.Return);   // 注入归还回调
            return fb;
        }
    }
}