using UnityEngine;

namespace MonsterSurvivors
{
    public class EnemyBehavior : MonoBehaviour
    {
        [Header("引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Rigidbody2D rigidbody2D;

        private EnemyData data;
        private float currentHP;
        private Transform player;   // 每帧自己找玩家，不依赖 Instance

        // 运行时最终属性（不污染 ScriptableObject）
        private float finalDamage;
        private float finalSpeed;

        public float CurrentHP => currentHP;
        public EnemyData Data => data;

        public void Init(EnemyData enemyData)
        {
            data = enemyData;

            // 应用关卡倍率
            float hpMult = 1f, dmgMult = 1f, spdMult = 1f;
            if (StageSelection.CurrentStage != null)
            {
                hpMult = StageSelection.CurrentStage.enemyHPMultiplier;
                dmgMult = StageSelection.CurrentStage.enemyDamageMultiplier;
                spdMult = StageSelection.CurrentStage.enemySpeedMultiplier;
            }

            currentHP = data.hp * hpMult;
            finalDamage = data.damage * dmgMult;
            finalSpeed = data.speed * spdMult;

            spriteRenderer.sprite = data.sprite;

        }

        private void Start()
        {
            if (PlayerBehavior.Instance != null)
                player = PlayerBehavior.Instance.transform;
        }

        private void Update()
        {
            // 没找到玩家就再试一次
            if (player == null)
            {
                if (PlayerBehavior.Instance != null)
                    player = PlayerBehavior.Instance.transform;
                return;
            }

            if (PlayerBehavior.Instance.IsDead) return;

            Vector2 direction = ((Vector2)player.position - rigidbody2D.position).normalized;

            if (direction.x > 0.1f) spriteRenderer.flipX = false;
            else if (direction.x < -0.1f) spriteRenderer.flipX = true;

            rigidbody2D.MovePosition(rigidbody2D.position + direction * finalSpeed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerBehavior.Instance.TakeDamage(finalDamage);
            }
        }

        public void TakeDamage(float damage)
        {
            currentHP -= damage;

            spriteRenderer.color = Color.red;
            EasingManager.Instance.DoAfter(0.07f, () =>
            {
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            });

            if (currentHP <= 0) Die();
        }

        private void Die()
        {
            // 死亡先掉落
            DropManager dropManager = FindFirstObjectByType<DropManager>();
            if (dropManager != null && data.drops != null)
            {
                foreach (var dropEntry in data.drops)
                {
                    if (Random.Range(0, 100f) < dropEntry.chance)
                    {
                        dropManager.Drop(dropEntry.dropType, transform.position);
                    }
                }
            }

            EnemySpawner spawner = FindFirstObjectByType<EnemySpawner>();
            if (spawner != null)
                spawner.Return(data, this);
            else
                Destroy(gameObject);
        }

    }
}
