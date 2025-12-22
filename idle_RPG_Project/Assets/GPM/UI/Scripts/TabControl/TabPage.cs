
namespace Gpm.Ui
{
    using DG.Tweening;
    using Internal;
    using UnityEngine;
    using UnityEngine.Events;


    public interface ITabPage
    {
        void Notify(Tab select);
    }

    public enum ETabActiveType
    {
        Immediate,
        SlidefromLeft,
        SlidefromRight,
        SlidefromTop,
        SlidefromBottom,
    }

    public class TabPage : TabLinkObject, ITabPage
    {
        [Header("Option", order = 0)]
        public ETabActiveType activeType = ETabActiveType.Immediate;

        [Header("Event", order = 2)]
        public NotifyTabPageEvent onNotify = new NotifyTabPageEvent();

        [SerializeField]private float duration = 0.3f;
        [SerializeField] private RectTransform panelRectTransform;

        public void Notify(Tab tab)
        {
            TabTransition(tab);
            onNotify.Invoke(tab);
        }

        private Vector2 GetOffsetPosition()
        {
            float width = panelRectTransform.rect.width;
            float height = panelRectTransform.rect.height;

            switch (activeType)
            {
                case ETabActiveType.SlidefromLeft: return new Vector2(-width, 0);
                case ETabActiveType.SlidefromRight: return new Vector2(width, 0);
                case ETabActiveType.SlidefromTop: return new Vector2(0, height);
                case ETabActiveType.SlidefromBottom: return new Vector2(0, -height);
                default: return Vector2.zero;
            }
        }

        private void TabTransition(Tab tab)
        {
            if (panelRectTransform == null)
            {
                panelRectTransform = GetComponent<RectTransform>();
            }

            // 1. 애니메이션 꼬임 방지: 실행 중인 트윈 즉시 중단
            panelRectTransform.DOKill();

            bool isSelected = tab.IsSelected();
            Vector2 targetPos = GetOffsetPosition();

            if (isSelected)
            {
                // [열기 로직]
                gameObject.SetActive(true); // 먼저 켜야 보입니다.

                if (activeType == ETabActiveType.Immediate)
                {
                    panelRectTransform.anchoredPosition = Vector2.zero;
                }
                else
                {
                    
                    panelRectTransform.anchoredPosition = targetPos;
                    // DOAnchorPos: RectTransform의 앵커 좌표를 이동시키는 DOTween 메서드
                    panelRectTransform.DOAnchorPos(Vector2.zero, duration)
                        .SetEase(Ease.OutQuad);
                }
            }
            else
            {
                // [닫기 로직]
                if (activeType == ETabActiveType.Immediate)
                {
                    gameObject.SetActive(false);
                }
                else
                {

                    panelRectTransform.DOAnchorPos(targetPos, duration)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                           gameObject.SetActive(false);
                        });
                }
            }
        }

        [System.Serializable]
        public class NotifyTabPageEvent : UnityEvent<Tab>
        {
            public NotifyTabPageEvent()
            {

            }
        }
    }
}
