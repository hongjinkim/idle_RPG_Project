using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Gpm.Ui
{
    
    using Internal;

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
        private RectTransform rectTransform;

        public void Notify(Tab tab)
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            // 1. 애니메이션 꼬임 방지: 실행 중인 트윈 즉시 중단
            rectTransform.DOKill();

            bool isSelected = tab.IsSelected();

            //if (isSelected)
            //{
            //    // [열기 로직]
            //    gameObject.SetActive(true); // 먼저 켜야 보입니다.

            //    if (activeType == ETabActiveType.Immediate)
            //    {
            //        rectTransform.anchoredPosition = Vector2.zero;
            //    }
            //    else
            //    {
            //        // 시작 위치로 순간 이동 후 -> (0,0)으로 이동
            //        Vector2 startPos = GetOffsetPosition();
            //        rectTransform.anchoredPosition = startPos;

            //        // DOAnchorPos: RectTransform의 앵커 좌표를 이동시키는 DOTween 메서드
            //        rectTransform.DOAnchorPos(Vector2.zero, duration)
            //            .SetEase(showEase);
            //    }
            //}
            //else
            //{
            //    // [닫기 로직]
            //    if (activeType == ETabActiveType.Immediate)
            //    {
            //        gameObject.SetActive(false);
            //    }
            //    else
            //    {
            //        // (0,0)에서 -> 목표 위치(화면 밖)로 이동
            //        Vector2 targetPos = GetOffsetPosition();

            //        rectTransform.DOAnchorPos(targetPos, duration)
            //            .SetEase(hideEase)
            //            .OnComplete(() =>
            //            {
            //                // 애니메이션이 다 끝난 뒤에 끕니다.
            //                gameObject.SetActive(false);
            //            });
            //    }
            //}

            onNotify.Invoke(tab);
        }

        private Vector2 GetOffsetPosition()
        {
            float width = rectTransform.rect.width;
            float height = rectTransform.rect.height;

            switch (activeType)
            {
                case ETabActiveType.SlidefromLeft: return new Vector2(-width, 0);
                case ETabActiveType.SlidefromRight: return new Vector2(width, 0);
                case ETabActiveType.SlidefromTop: return new Vector2(0, height);
                case ETabActiveType.SlidefromBottom: return new Vector2(0, -height);
                default: return Vector2.zero;
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
