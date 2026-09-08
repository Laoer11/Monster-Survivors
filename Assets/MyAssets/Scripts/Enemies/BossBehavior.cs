using UnityEngine;

namespace MonsterSurvivors
{
    public class BossBehavior : MonoBehaviour
    {
        [Header("引用")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Rigidbody2D rigidbody2D;

        private BossData data;
        private float currentHP;
        private float maxHP;
        private Transform player;
        private int currentPhase = 1;

        public float CurrentHP => currentHP;
        public float MaxHP => maxHP;
        public float HPPercent => currentHP / maxHP;
        public BossData Data => data;

        public System.Action<BossBehavior> onBossDied;

        public void Init(BossData bossData)
        {
            data = bossData;
            maxHP = data.hp;
            currentHP = maxHP;
            spriteRenderer.sprite = data.sprite;
            transform.localScale = Vector3.one * 2f;    // 大小翻倍
        }

        private void Start()
        {
            if(PlayerBehavior.Instance != null)
                player = PlayerBehavior.Instance.transform;

            //通知UI显示Boss血条
            BossHealthBarUI.Show(this);
        }

        private void Update()
        {
            if(player == null) return;
            if(PlayerBehavior.Instance.IsDead) return;

            Vector2 direction = ((Vector2)player.position - rigidbody2D.position).normalized;

            if(direction.x > 0.1f) spriteRenderer.flipX = false;
            else if(direction.x < -0.1f) spriteRenderer.flipX = true;

            float speed = data.speed;
            if(currentPhase >= 2) speed *= data.phase2SpeedMultiplier;

            rigidbody2D.MovePosition(rigidbody2D.position + direction * speed * Time.fixedDeltaTime);

            //距离检测 : 撞到玩家
            if(Vector2.Distance(rigidbody2D.position,player.position) < 0.6f)
            {
                PlayerBehavior.Instance.TakeDamage(data.damage);
            }
        }

        public void TakeDamage(float damage)
        {
            currentHP -= damage;

            //闪白
            spriteRenderer.color = Color.red;
            EasingManager.Instance.DoAfter(0.07f, () =>
            {
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            });

            // 阶段切换
            CheckPhase();

            if (currentHP <= 0) Die();
        }

        private void CheckPhase()
        {
            if (currentPhase == 1 && HPPercent <= data.phase2Threshold)
            {
                currentPhase = 2;
                spriteRenderer.color = Color.red;   // 狂暴变红
                Debug.Log($"[Boss] 进入第 2 阶段！速度 ×{data.phase2SpeedMultiplier}");
            }
        }

        private void Die()
        {
            BossHealthBarUI.Hide();
            onBossDied?.Invoke(this);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            BossHealthBarUI.Hide();
        }
    }
}