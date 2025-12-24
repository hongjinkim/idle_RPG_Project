using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;

public class FXManager : PoolManager<FXManager, EPoolType>
{
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private Camera effectCamera;
    private Camera _mainCamera;

    private Dictionary<HPBar, Transform> _activeBars = new Dictionary<HPBar, Transform>();
    public Vector3 HpBarOffset = new Vector3(0, 80f, 0); // HP바 오프셋

    protected override async UniTask OnInitialize()
    {
        await base.OnInitialize();
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        // 활성화된 모든 HP바 위치 갱신
        foreach (var kvp in _activeBars)
        {
            HPBar bar = kvp.Key;
            Transform target = kvp.Value;

            if (target == null || !target.gameObject.activeInHierarchy)
            {
                // 타겟이 사라지면(비활성/파괴) 안전하게 무시 (반납은 CharacterBase에서 호출됨)
                bar.gameObject.SetActive(false);
                continue;
            }
            if(bar.canvasGroup.alpha <=0)
            {
                // 투명 상태면 위치 갱신 안함
                continue;
            }
            UpdatePosition(bar, target.position + HpBarOffset);
        }

    }

    private void UpdatePosition(HPBar bar, Vector3 worldPos)
    {
        // 월드 좌표(적 위치) -> 스크린 좌표(화면 픽셀) 변환 (Main Camera 기준)
        Vector3 screenPos = _mainCamera.WorldToScreenPoint(worldPos);

        // 화면 뒤쪽(카메라 뒤)에 있는 경우 숨김 처리
        if (screenPos.z < 0)
        {
            bar.gameObject.SetActive(false);
            return;
        }
        else if (!bar.gameObject.activeSelf)
        {
            bar.gameObject.SetActive(true);
        }

        // 스크린 좌표 -> 캔버스 로컬 좌표 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,     // 기준이 될 캔버스(부모)
            screenPos,      // 변환할 스크린 좌표
            effectCamera,       // UI를 찍는 카메라 (Overlay Camera)
            out localPoint  // 변환된 좌표가 담길 변수
        );

        bar.RectTransform.anchoredPosition = localPoint;
    }

    public GameObject GetDamageText(Vector3 position, AttackInfo attackInfo)
    {
        var obj = Pop(EPoolType.DamageText, position);
        obj.GetComponentInChildren<DamageTextEffect>().SetDamage(attackInfo);
        return obj;
    }

    public HPBar GetHpBar(Transform targetTransform, bool isAlwaysVisible)
    {
        var obj = Pop(EPoolType.HpBar);
        var hpBar = obj.GetComponentInChildren<HPBar>();

        hpBar.Initialize(isAlwaysVisible);
        _activeBars.Add(hpBar, targetTransform);

        UpdatePosition(hpBar, targetTransform.position + HpBarOffset);

        return hpBar;
    }

    public GameObject GetLootItem(Vector3 position)
    {
        var obj = Pop(EPoolType.LootItem, position);
        return obj;
    }

    public void ReturnHpBar(HPBar hpBar)
    {
        if (_activeBars.ContainsKey(hpBar))
        {
            _activeBars.Remove(hpBar);
        }
        hpBar.CleanUp();
    }
}
