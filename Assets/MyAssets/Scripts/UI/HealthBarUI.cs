using UnityEngine;
using UnityEngine.UI;

namespace MonsterSurvivors
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private PlayerBehavior player;

        private float lastHP;

        private void Start()
        {
            if (player == null) player = FindFirstObjectByType<PlayerBehavior>();
            lastHP = player.MaxHP;
        }

        private void Update()
        {
            if (player == null) return;

            float percent = player.CurrentHP / player.MaxHP;
            fillImage.fillAmount = percent;
        }
    }
}
