using UnityEngine;
using TMPro;

namespace MonsterSurvivors
{
    /// <summary>
    /// 金币显示器 —— 拖到任何 Canvas 下即可工作
    /// </summary>
    public class GoldDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private float updateDelay = 0.1f;

        private int displayGold;
        private float lastUpdateTime;

        private void Start()
        {
            var save = GameController.SaveManager.GetSave<PlayerSave>("PlayerSave");
            displayGold = save.gold;
            goldText.text = displayGold.ToString();
        }

        private void Update()
        {
            var save = GameController.SaveManager.GetSave<PlayerSave>("PlayerSave");

            //金币没变就不刷新
            if(save.gold == displayGold) return;

            //延迟刷新,避免频繁刷新
            if (Time.unscaledTime - lastUpdateTime < updateDelay) return;
            lastUpdateTime = Time.unscaledTime;

            displayGold = save.gold;
            goldText.text = displayGold.ToString();
        }
    }
}