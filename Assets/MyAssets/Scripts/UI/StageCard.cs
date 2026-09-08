using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MonsterSurvivors
{
    public class StageCard : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image previewImage;
        [SerializeField] private TMP_Text stageNameText;
        [SerializeField] private TMP_Text stageDescText;
        [SerializeField] private Button btnPlay;
        [SerializeField] private GameObject lockOverlay;  // 未解锁时的遮罩

        private StageData data;

        public void Setup(StageData stageData)
        {
            data = stageData;
            previewImage.sprite = stageData.previewSprite;
            stageNameText.text = stageData.stageName;
            stageDescText.text = stageData.description;

            bool unlocked = stageData.isUnlocked;

            btnPlay.gameObject.SetActive(unlocked);
            btnPlay.onClick.AddListener(OnPlayClicked);

            if(lockOverlay != null) lockOverlay.SetActive(!unlocked);
        }

        private void OnPlayClicked()
        {
            AudioManager.PlaySFX("Click");
            StageSelection.CurrentStage = data;   
            Debug.Log($"[StageCard] 选择关卡: {data.stageName}");
            GameController.LoadStage(data.sceneName);
        }
    }
}