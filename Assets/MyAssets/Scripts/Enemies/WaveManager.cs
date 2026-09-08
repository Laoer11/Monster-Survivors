using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    public class WaveManager : MonoBehaviour
    {
        [Header("数据")]
        [SerializeField] private WavesDatabase wavesDatabase;
        [SerializeField] private List<EnemyData> enemyPool;

        [Header("生成器")]
        [SerializeField] private EnemySpawner spawner;

        [Header("UI")]
        [SerializeField] private TMPro.TMP_Text waveText;

        [Header("Boss")]
        [SerializeField] private BossData bossData;
        [SerializeField] private int bossWaveInterval = 5;

        [Header("无限波次参数")]
        [SerializeField] private int baseEnemyCount = 5;
        [SerializeField] private int extraPerWave = 3;
        [SerializeField] private float minInterval = 0.3f;
        [SerializeField] private float intervalDecrease = 0.05f;

        private int waveNumber;

        // 关卡时间对外暴露
        public float StageDuration { get; private set; }
        public float ElapsedTime { get; private set; }

        private void Start()
        {
            StartCoroutine(RunWaves());
        }

        private IEnumerator RunWaves()
        {
            // 局部变量改为成员变量
            StageDuration = 60f;
            if (StageSelection.CurrentStage != null)
                StageDuration = StageSelection.CurrentStage.duration;
            ElapsedTime = 0f;

            // 循环条件改为成员变量
            while (ElapsedTime < StageDuration)
            {
                waveNumber++;

                if (waveText != null)
                    waveText.text = $"第 {waveNumber} 波  |  剩余 {Mathf.CeilToInt(StageDuration - ElapsedTime)}s";

                int count;
                float interval;
                float delay;

                if (waveNumber <= wavesDatabase.Count)
                {
                    WaveData w = wavesDatabase.GetWave(waveNumber - 1);
                    count = w.totalCount;
                    interval = w.spawnInterval;
                    delay = w.spawnDelay;
                }
                else
                {
                    count = baseEnemyCount + (waveNumber - 1) * extraPerWave;
                    interval = Mathf.Max(minInterval, 1.5f - (waveNumber - 1) * intervalDecrease);
                    delay = 3f;
                }

                yield return new WaitForSeconds(delay);
                ElapsedTime += delay;   // 累加成员变量

                for (int i = 0; i < count; i++)
                {
                    SpawnRandomEnemy();
                    yield return new WaitForSeconds(interval);
                    ElapsedTime += interval;   // 累加成员变量
                }

                if (waveNumber % bossWaveInterval == 0)
                {
                    yield return new WaitForSeconds(1f);
                    ElapsedTime += 1f;   // 累加成员变量

                    Vector2 pos = spawner.GetRandomSpawnPosition();
                    GameObject go = Instantiate(bossData.prefab, pos, Quaternion.identity);
                    go.GetComponent<BossBehavior>().Init(bossData);
                }

                yield return new WaitForSeconds(3f);
                ElapsedTime += 3f;   // 累加成员变量
            }

            Debug.Log("[WaveManager] 关卡时间到，胜利！");
            Time.timeScale = 0;
        }

        private void SpawnRandomEnemy()
        {
            EnemyData data;

            if (waveNumber <= wavesDatabase.Count)
                data = wavesDatabase.GetWave(waveNumber - 1).enemyData;
            else
                data = enemyPool[Random.Range(0, enemyPool.Count)];

            Vector2 pos = spawner.GetRandomSpawnPosition();
            EnemyBehavior enemy = spawner.Get(data);
            enemy.transform.position = pos;
        }
    }
}
