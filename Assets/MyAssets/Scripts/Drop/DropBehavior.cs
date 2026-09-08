using UnityEngine;

namespace MonsterSurvivors
{
    public class DropBehavior : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private new Rigidbody2D rigidbody2D;

        private DropData data;
        private float magnetSpeed = 8f; //吸引速度

        public void Init(DropData dropData)
        {
            data = dropData;
            spriteRenderer.sprite = data.sprite;
        }

        private void FixedUpdate()   // ★ 改 FixedUpdate
        {
            if (PlayerBehavior.Instance == null) return;

            float distance = Vector2.Distance(transform.position, PlayerBehavior.Instance.transform.position);
            if (distance < PlayerBehavior.Instance.MagnetRadius)
            {
                rigidbody2D.MovePosition(
                    Vector2.MoveTowards(
                        rigidbody2D.position,
                        PlayerBehavior.Instance.transform.position,
                        magnetSpeed * Time.fixedDeltaTime
                    )
                );
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(other.gameObject.layer != LayerMask.NameToLayer("Player")) return;

            // 触发效果
            ApplyEffect();
            Destroy(gameObject);
        }

        private void ApplyEffect()
        {
            switch (data.dropType)
            {
                case DropType.Coin:
                    var playerSave = GameController.SaveManager.GetSave<PlayerSave>("PlayerSave");
                    playerSave.gold += (int)data.value;
                    Debug.Log($"[Drop] 拾取金币 +{data.value}，当前金币 {playerSave.gold}");
                    break;

                case DropType.Food:
                    PlayerBehavior.Instance.Heal(data.value);
                    break;

                case DropType.Gem:
                    var xpManager = FindFirstObjectByType<ExperienceManager>();
                    if (xpManager != null)
                        xpManager.AddXP(data.value);
                    break;
            }
        }
    }
}