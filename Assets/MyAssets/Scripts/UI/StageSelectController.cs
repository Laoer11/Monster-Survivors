using UnityEngine;
using UnityEngine.UI;

namespace MonsterSurvivors.UI
{
    public class StageSelectController : MonoBehaviour
    {
        [Header("数据")]
        [SerializeField] private StagesDatabase stagesDatabase;
        [SerializeField] private StageCard stageCardPrefab;

        [Header("滚动容器")]
        [SerializeField] private RectTransform stagesContainer;
        [SerializeField] private RectTransform viewport;

        [Header("动画")]
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("箭头")]
        [SerializeField] private Button btnLeft;
        [SerializeField] private Button btnRight;

        [Header("返回")]
        [SerializeField] private Button btnBack;

        private int currentIndex = 0;
        private StageCard[] cards;

        private void Start()
        {
            btnBack.onClick.AddListener(OnBackClicked);
            btnLeft.onClick.AddListener(() => ShowCard(-1));
            btnRight.onClick.AddListener(() => ShowCard(1));

            GenerateStageCards();
            for(int i=0;i<cards.Length;i++)
            {
                cards[i].gameObject.SetActive(i==0);
            }
            PlayEnterAnimation();
        }

        private void GenerateStageCards()
        {
            float cardWidth = viewport.rect.width;

            cards = new StageCard[stagesDatabase.Count];

            for (int i = 0; i < stagesDatabase.Count; i++)
            {
                StageCard card = Instantiate(stageCardPrefab, stagesContainer);
                card.Setup(stagesDatabase.stages[i]);

                RectTransform cardRect = card.transform as RectTransform;
                cardRect.sizeDelta = new Vector2(cardWidth, cardRect.sizeDelta.y);
                cardRect.anchorMin = new Vector2(0.5f, 0.5f);
                cardRect.anchorMax = new Vector2(0.5f, 0.5f);
                cardRect.anchoredPosition = Vector2.zero;

                cards[i] = card;
            }
        }

        private void ShowCard(int direction)
        {
            int targetIndex = currentIndex + direction;
            if (targetIndex < 0 || targetIndex >= cards.Length) return;

            currentIndex = targetIndex;

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].gameObject.SetActive(i == currentIndex);
            }
        }

        private void PlayEnterAnimation()
        {
            canvasGroup.alpha = 0f;
            EasingManager.Instance.RunAnimation(FadeIn());
        }

        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            while (elapsed < 0.4f)
            {
                elapsed += Time.deltaTime;
                if (canvasGroup == null) yield break;
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / 0.4f);
                yield return null;
            }
            if (canvasGroup != null) canvasGroup.alpha = 1f;
        }

        private void OnBackClicked()
        {
            AudioManager.PlaySFX("Click");
            GameController.LoadLobby();
        }
    }
}