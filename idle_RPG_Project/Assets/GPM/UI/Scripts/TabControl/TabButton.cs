using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Gpm.Ui
{
    public class TabButton : Tab, IPointerClickHandler
    {
        [SerializeField] private LayoutElement le;
        [SerializeField] private RectTransform rt;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnClick();
        }

        public void HighlightSelectedTab()
        {
            if (le == null)
            {
                le = GetComponent<LayoutElement>();
            }
            if(rt == null)
            {
                rt = GetComponent<RectTransform>();
            }


            float targetFlexibleWidth = selected ? 1.4f : 1f;
            DOTween.To(() => le.flexibleWidth,
                       v => le.flexibleWidth = v,
                       targetFlexibleWidth,
                       0.2f)
                   .SetEase(Ease.OutCubic);

            float targetHeight = selected ? 250f : 216f; // 원하는 높이 값으로 변경
            DOTween.To(() => rt.sizeDelta.y,
                       v => rt.sizeDelta = new Vector2(rt.sizeDelta.x, v),
                       targetHeight,
                       0.2f)
                   .SetEase(Ease.OutCubic);
        }

    }
}
