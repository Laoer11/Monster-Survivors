using System.Collections.Generic;
using UnityEngine;

namespace MonsterSurvivors
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("设置")]
        [SerializeField] private Transform player;
        [SerializeField] private float spawnDistance = 10f; //在玩家多远处生成

        private Dictionary<string,Queue<EnemyBehavior>> pools;

        private void Awake()
        {
            pools = new Dictionary<string,Queue<EnemyBehavior>>();
        }

        private void Start()
        {
            if(player == null)
                player = PlayerBehavior.Instance.transform;
        }

        /// <summary>
        /// 从池中取一个敌人，没有就创建
        /// </summary>
        public EnemyBehavior Get(EnemyData data)
        {
            string key = data.enemyId;

            if(!pools.ContainsKey(key))
                pools[key] = new Queue<EnemyBehavior>();

            EnemyBehavior enemy;
            if(pools[key].Count > 0)
            {
                enemy = pools[key].Dequeue();
                enemy.gameObject.SetActive(true);
            }
            else
            {
                GameObject go = Instantiate(data.prefab);
                enemy = go.GetComponent<EnemyBehavior>();
            }

            enemy.Init(data);
            return enemy;
        }

        /// <summary>
        /// 把敌人归还池中
        /// </summary>
        public void Return(EnemyData data,EnemyBehavior enemy)
        {
            enemy.gameObject.SetActive(false);
            pools[data.enemyId].Enqueue(enemy);
        }
        
        /// <summary>
        /// 在屏幕外随机位置生成
        /// </summary>
        public Vector2 GetRandomSpawnPosition()
        {
            Camera cam = Camera.main;
            float halfH = cam.orthographicSize;
            float halfW = halfH * cam.aspect;
            Vector2 camPos = cam.transform.position;
            float padding = 2f;

            if (Random.value > 0.5f)
            {
                float x = Random.value > 0.5f ? camPos.x + halfW + padding : camPos.x - halfW - padding;
                return new Vector2(x, Random.Range(camPos.y - halfH, camPos.y + halfH));
            }
            else
            {
                float y = Random.value > 0.5f ? camPos.y + halfH + padding : camPos.y - halfH - padding;
                return new Vector2(Random.Range(camPos.x - halfW, camPos.x + halfW), y);
            }

        }
    }
}