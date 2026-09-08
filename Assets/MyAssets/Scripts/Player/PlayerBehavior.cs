using UnityEngine;

namespace MonsterSurvivors
{
    public class PlayerBehavior : MonoBehaviour
    {
        // 单例，方便其他系统访问
        public static PlayerBehavior Instance { get; private set; }

        [Header("数据")]
        [SerializeField] private CharactersDatabase charactersDatabase;

        [Header("引用")]
        [SerializeField] private new Rigidbody2D rigidbody2D;
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("受伤")]
        [SerializeField] private Color hitFlashColor = Color.red;
        [SerializeField] private float hitFlashDuration = 0.1f;
        [SerializeField] private float invincibleDuration = 2f;

        // 事件：冷却倍率变化时通知技能行为
        public static System.Action<float> CooldownMultiplierChanged;

        // ===== 基础属性（从 CharacterData 加载） =====
        private CharacterData characterData;
        private float baseSpeed;
        private float baseDamage;
        private float baseMagnetRadius;

        // ===== 当前倍率（被动技能/升级修改） =====
        private float moveSpeedMultiplier = 1f;
        private float damageMultiplier = 1f;
        private float maxHPMultiplier = 1f;
        private float magnetMultiplier = 1f;
        private float xpMultiplier = 1f;
        private float cooldownMultiplier = 1f;
        private float damageReductionPercent = 0f;
        private float projectileSpeedMultiplier = 1f;
        private float sizeMultiplier = 1f;
        private float durationMultiplier = 1f;
        private float goldMultiplier = 1f;

        // ===== 最终计算值 =====
        public float MoveSpeed { get; private set; }
        public float Damage { get; private set; }
        public float MagnetRadius { get; private set; }
        public float CurrentHP { get; private set; }
        public float MaxHP { get; private set; }

        // ===== 对外暴露的倍率（只读） =====
        public float XPMultiplier => xpMultiplier;
        public float CooldownMultiplier => cooldownMultiplier;
        public float DamageReductionMultiplier => (100f - damageReductionPercent) / 100f;
        public float ProjectileSpeedMultiplier => projectileSpeedMultiplier;
        public float SizeMultiplier => sizeMultiplier;
        public float DurationMultiplier => durationMultiplier;
        public float GoldMultiplier => goldMultiplier;
        public Vector2 LookDirection => movement.normalized;

        public bool IsDead => CurrentHP <= 0;
        private bool isInvincible;
        private Vector2 movement;
        private EasingCoroutine flashCoroutine;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            Instance = this;

            if (rigidbody2D == null) rigidbody2D = GetComponent<Rigidbody2D>();
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            Debug.Log($"[Player] charactersDatabase 是否为null: {charactersDatabase == null}");  

            // ★ 从存档读取当前选择的角色
            var charSave = GameController.SaveManager.GetSave<CharactersSave>("Characters");
            characterData = charactersDatabase.GetById(charSave.selectedCharacterId);
            if (characterData == null) characterData = charactersDatabase.characters[0];

            baseSpeed = characterData.baseSpeed;
            baseDamage = characterData.baseDamage;
            baseMagnetRadius = characterData.baseMagnetRadius;

            // ★ 加上永久升级
            damageReductionPercent = characterData.initialDamageReductionPercent;
            xpMultiplier = characterData.defaultXPMultiplier;
            cooldownMultiplier = characterData.defaultCooldownMultiplier;
            projectileSpeedMultiplier = characterData.defaultProjectileSpeed;
            sizeMultiplier = characterData.defaultSizeMultiplier;
            durationMultiplier = characterData.defaultDurationMultiplier;
            goldMultiplier = characterData.defaultGoldMultiplier;

            // ★ 计算初始属性
            RecalculateAll();
            CurrentHP = MaxHP;
        }

        // ★★★ 一键重算所有属性 ★★★
        public void RecalculateAll()
        {
            RecalculateMoveSpeed();
            RecalculateDamage();
            RecalculateMaxHP();
            RecalculateMagnetRadius();
        }

        // ★★★ 各属性重算方法 ★★★
        public void RecalculateMoveSpeed()
        {
            MoveSpeed = baseSpeed * moveSpeedMultiplier;
            // 叠加永久升级
            // if (GameController.UpgradesManager != null)
            //     MoveSpeed += GameController.UpgradesManager.GetValue(UpgradeType.MoveSpeed) * 0.5f;

        }

        public void RecalculateDamage()
        {
            Damage = baseDamage * damageMultiplier;
            if (GameController.UpgradesManager != null)
                Damage += GameController.UpgradesManager.GetValue(UpgradeType.Damage);

        }

        public void RecalculateMaxHP()
        {
            float oldMax = MaxHP;
            MaxHP = characterData.baseHP * maxHPMultiplier;
            if (GameController.UpgradesManager != null)
                MaxHP += GameController.UpgradesManager.GetValue(UpgradeType.Health);


            // HP 上限变了，当前 HP 跟着等比缩放
            if (oldMax > 0)
                CurrentHP = CurrentHP / oldMax * MaxHP;
        }

        public void RecalculateMagnetRadius()
        {
            MagnetRadius = baseMagnetRadius * magnetMultiplier;
        }

        // 倍率修改接口（被动技能调用）
        public void ApplyMoveSpeedMultiplier(float mult) { moveSpeedMultiplier *= mult; RecalculateMoveSpeed(); }
        public void ApplyDamageMultiplier(float mult) { damageMultiplier *= mult; RecalculateDamage(); }
        public void ApplyMaxHPMultiplier(float mult) { maxHPMultiplier *= mult; RecalculateMaxHP(); }
        public void ApplyMagnetMultiplier(float mult) { magnetMultiplier *= mult; RecalculateMagnetRadius(); }
        public void ApplyCooldownMultiplier(float mult) {cooldownMultiplier *= mult;CooldownMultiplierChanged?.Invoke(CooldownMultiplier); }


        private void Update()
        {
            if (IsDead) return;

            movement = InputManager.Movement;
            animator.SetFloat(SpeedHash, movement.magnitude);

            if (movement.x > 0.01f) spriteRenderer.flipX = false;
            else if (movement.x < -0.01f) spriteRenderer.flipX = true;

            // 测试：空格扣血
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                TakeDamage(10f);
            }

            // ★ 测试：按 1 键回血
            if (UnityEngine.InputSystem.Keyboard.current != null &&
                UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Heal(20f);
            }
        }

        private void FixedUpdate()
        {
            if (IsDead) return;
            rigidbody2D.MovePosition(rigidbody2D.position + movement * MoveSpeed * Time.fixedDeltaTime);
        }

        public void TakeDamage(float damage)
        {
            if (isInvincible || IsDead) return;

            float finalDamage = damage * DamageReductionMultiplier;
            CurrentHP -= finalDamage;
            CurrentHP = Mathf.Max(0, CurrentHP);
            Debug.Log($"[Player] 受到 {finalDamage:F1} 伤害, HP: {CurrentHP}/{MaxHP}");

            FlashHit();

            isInvincible = true;
            EasingManager.Instance.DoAfter(invincibleDuration, () => isInvincible = false);

            if (IsDead) Die();
        }

        // ★★★ 回血 ★★★
        public void Heal(float amount)
        {
            CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
            Debug.Log($"[Player] 回血 {amount}, HP: {CurrentHP}/{MaxHP}");
        }

        private void FlashHit()
        {
            flashCoroutine?.Stop();
            spriteRenderer.color = hitFlashColor;
            EasingManager.Instance.DoAfter(hitFlashDuration, () =>
            {
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            });
        }

        private void Die()
        {
            Debug.Log("[Player] 玩家死亡！");
            animator.SetFloat(SpeedHash, 0f);
            this.enabled = false;
        }
    }
}
