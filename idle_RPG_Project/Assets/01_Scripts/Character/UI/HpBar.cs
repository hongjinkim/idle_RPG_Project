using DG.Tweening;
using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image frontHpImage;
    [SerializeField] private Image backHpImage;  
    public CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float ghostDelay = 0.5f;    // 맞고나서 줄어들기 시작할 대기 시간
    [SerializeField] private float ghostDuration = 0.5f; // 줄어드는 시간
    private bool _isAlwaysVisible = false; // 항상 보이게 할지 여부

    private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform; // 매니저 접근용

    private Poolable _poolable;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _poolable = GetComponent<Poolable>();
    }

    public void Initialize(BigInteger currentHp, BigInteger maxHp, bool alwaysVisible)
    {
        _isAlwaysVisible = alwaysVisible;

        float ratio = (float)currentHp / (float)maxHp;

        // 초기화: 둘 다 꽉 채움
        frontHpImage.fillAmount = ratio;
        backHpImage.fillAmount = ratio;

        if (_isAlwaysVisible)
        {
            canvasGroup.alpha = 1f; // 항상 보임
        }
        else
        {
            // 최대 체력이면 숨김, 아니면 보임
            bool isFullHp = ratio >= 0.99f;
            canvasGroup.alpha = isFullHp ? 0f : 1f;
        }

        gameObject.SetActive(true);
    }

    public void SetHP(BigInteger currentHp, BigInteger maxHp)
    {
        float targetRatio = (float)currentHp / (float)maxHp;

        // 1. 앞쪽 체력바는 즉시 반영
        frontHpImage.fillAmount = targetRatio;

        // 2. 뒤쪽(Ghost) 체력바 연출
        // 실행 중이던 트윈이 있다면 즉시 중단 (현재 fillAmount에서 멈춤)
        backHpImage.DOKill();

        // 현재 backHpImage.fillAmount에서 targetRatio까지 줄어듬
        // 딜레이 후 부드럽게 감소
        backHpImage.DOFillAmount(targetRatio, ghostDuration)
            .SetDelay(ghostDelay)
            .SetEase(Ease.OutQuad);

        if (_isAlwaysVisible)
        {
            // 항상 보이는 녀석은 투명도 건드리지 않음 (혹은 강제로 1)
            if (canvasGroup.alpha < 1) canvasGroup.alpha = 1f;
        }
        else
        {
            // 조건부 표시 (기존 로직)
            if (targetRatio >= 0.99f)
            {
                if (canvasGroup.alpha > 0)
                {
                    canvasGroup.DOKill();
                    canvasGroup.DOFade(0f, 0.5f);
                }
            }
            else
            {
                if (canvasGroup.alpha < 1)
                {
                    canvasGroup.DOKill();
                    canvasGroup.alpha = 1f;
                }
            }
        }
    }

    public void CleanUp()
    {
        // 반납 시 트윈 킬 (메모리 누수 방지)
        backHpImage.DOKill();
        canvasGroup.DOKill();
        _poolable.Release();
    }
}