using UnityEngine;

namespace MonsterSurvivors
{
    /// <summary>
    /// 投射物基类 —— 飞行、碰撞、命中特效
    /// </summary>
    public class ProjectileBehavior : MonoBehaviour
    {
        [Header("基础组件")]
        [SerializeField] protected SpriteRenderer spriteRenderer;
        [SerializeField] protected new Rigidbody2D rigidbody2D;

        [Header("命中特效")]
        [SerializeField] protected GameObject hitVFXPrefab;

        protected float speed;
        protected float damage;
        protected float lifetime;
        protected Vector2 direction;
        protected int pierceCount;     // 穿透次数（0 = 不穿透，碰到就消失）

        // 归还到对象池的回调
        private System.Action<ProjectileBehavior> returnToPool;

        /// <summary>
        /// 注入归还回调。Get 之后、Launch 之前调用。
        /// </summary>
        public void SetReturnCallback(System.Action<ProjectileBehavior> callback)
        {
            returnToPool = callback;
        }

        public virtual void Launch(Vector2 dir, float spd, float dmg, float life, int pierce = 0)
        {
            direction = dir;
            speed = spd;
            damage = dmg;
            lifetime = life;
            pierceCount = pierce;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            Invoke(nameof(Expire), lifetime);
        }

        protected virtual void FixedUpdate()
        {
            rigidbody2D.MovePosition(rigidbody2D.position + direction * speed * Time.fixedDeltaTime);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            EnemyBehavior enemy = other.GetComponent<EnemyBehavior>();
            if (enemy == null) return;

            enemy.TakeDamage(damage);
            SpawnHitVFX();

            if (pierceCount <= 0)
            {
                Expire();
            }
            else
            {
                pierceCount--;
            }
        }

        protected void SpawnHitVFX()
        {
            if (hitVFXPrefab != null)
            {
                GameObject vfx = Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }
        }

        protected virtual void Expire()
        {
            CancelInvoke();

            // 优先归还到池；未注入回调时走老逻辑
            if (returnToPool != null)
                returnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
}
