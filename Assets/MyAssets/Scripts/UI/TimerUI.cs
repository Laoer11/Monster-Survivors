using UnityEngine;
using TMPro;

namespace MonsterSurvivors
{
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

        private WaveManager waveManager;

        private void Start()
        {
            waveManager = FindFirstObjectByType<WaveManager>();
        }

        private void Update()
        {
            if(waveManager == null || timerText == null) return;

            float remaining = waveManager.StageDuration - waveManager.ElapsedTime;
            if(remaining < 0) remaining = 0;

            timerText.text = $"当前波次剩余时间: {Mathf.CeilToInt(remaining)}s";
        }
    }
}