using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    protected virtual void OnEnable()
    {
        // 1. 이벤트 구독 (자식 클래스에서 구현)
        BindEvents();

        // 2. 켜지는 순간 최신 데이터로 화면 갱신
        RefreshUI();
    }

    protected virtual void OnDisable()
    {
        // 3. 이벤트 구독 해제
        UnbindEvents();
    }

    // 자식들이 무조건 구현해야 하는 함수들
    public abstract void BindEvents();
    protected abstract void UnbindEvents();
    public abstract void RefreshUI();
}