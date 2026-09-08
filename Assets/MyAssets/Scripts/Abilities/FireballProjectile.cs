using UnityEngine;

namespace MonsterSurvivors
{
    public class FireballProjectile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Collider2D collider2D;
        [SerializeField] private new Rigidbody2D rigidbody2D;

        private float speed;
        private float damage;
        private float lifetime;
        private Vector2 direction;

        // 归还到对象池的回调（由 AbilityBehavior 在 Get 后注入）
        private System.Action<FireballProjectile> returnToPool;

        [Header("VFX")]
        [SerializeField] private GameObject hitVFXPrefab;

        /// <summary>
        /// 注入归还回调。Get 之后、Launch 之前调用。
        /// </summary>
        public void SetReturnCallback(System.Action<FireballProjectile> callback)
        {
            returnToPool = callback;
        }

        public void Launch(Vector2 dir,float spd,float dmg,float life)
        {
            direction = dir;
            speed = spd;
            damage = dmg;
            lifetime = life;

            // 旋转朝向飞行方向
            float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0,0,angle - 90);  // 旋转90度，使方向与精灵方向一致

            Invoke(nameof(Expire), lifetime);
        }

        private void FixedUpdate()
        {
            rigidbody2D.MovePosition(rigidbody2D.position + direction * speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.CompareTag("Enemy"))
            {
                EnemyBehavior enemy = other.GetComponent<EnemyBehavior>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
                Expire();
            }
        }

        private void Expire()
        {
            CancelInvoke();

            // 生成命中特效
            if(hitVFXPrefab != null)
            {
                GameObject vfx = Instantiate(hitVFXPrefab, transform.position, Quaternion.identity);
                Destroy(vfx, 1f);
            }

            // 优先归还到池；未注入回调时走老逻辑（兼容直接 Instantiate 的场景）
            if (returnToPool != null)
                returnToPool(this);
            else
                gameObject.SetActive(false);
        }
    }
}