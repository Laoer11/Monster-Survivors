using System.Collections;
using UnityEngine;

namespace MonsterSurvivors
{
    public class IceShardAbilityBehavior : MonoBehaviour, IAbilityBehavior
    {
        [Header("冰锥预制体")]
        [SerializeField] private ProjectileBehavior iceShardPrefab;

        [Header("每级参数")]
        [SerializeField] private float[] cooldowns = { 3.5f, 3f, 2.5f, 2f, 1.5f };
        [SerializeField] private float[] damages = { 8f, 12f, 18f, 27f, 40f };
        [SerializeField] private float[] speeds = { 5f, 6f, 7f, 8f, 9f };
        [SerializeField] private float lifetime = 3f;
        [SerializeField] private int pierce = 2;    // 穿透 2 个敌人

        private int level = 0;
        private Coroutine fireCoroutine;
        private Pool<ProjectileBehavior> pool;

        public void Init(int abilityLevel)
        {
            level = abilityLevel;

            pool = new Pool<ProjectileBehavior>(iceShardPrefab, 5, "IceShardPool", false);

            fireCoroutine = StartCoroutine(FireLoop());
        }

        public void Upgrade(int newLevel)
        {
            level = newLevel;
            if (fireCoroutine != null) StopCoroutine(fireCoroutine);
            fireCoroutine = StartCoroutine(FireLoop());
        }

        private IEnumerator FireLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(cooldowns[level]);

                EnemyBehavior target = FindNearestEnemy();
                if (target == null) continue;

                ProjectileBehavior shard = GetIceShard();
                shard.transform.position = PlayerBehavior.Instance.transform.position;

                Vector2 dir = (target.transform.position - shard.transform.position).normalized;
                shard.Launch(dir, speeds[level], damages[level], lifetime, pierce);
            }
        }

        private EnemyBehavior FindNearestEnemy()
        {
            EnemyBehavior[] all = FindObjectsByType<EnemyBehavior>(FindObjectsSortMode.None);
            EnemyBehavior nearest = null;
            float minDist = float.MaxValue;
            Vector2 playerPos = PlayerBehavior.Instance.transform.position;

            foreach (var e in all)
            {
                float dist = Vector2.SqrMagnitude((Vector2)e.transform.position - playerPos);
                if (dist < minDist) { minDist = dist; nearest = e; }
            }
            return nearest;
        }

        private ProjectileBehavior GetIceShard()
        {
            ProjectileBehavior shard = pool.Get();
            shard.SetReturnCallback(pool.Return);
            return shard;
        }

    }
}