using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public enum EEventType
{
    // 플레이어 관련
    PlayerHealthChanged,    // 체력 변경 
    PlayerExpChanged,       // 경험치 변경
    PlayerLevelUp,          // 레벨

    // 재화 관련
    GoldChanged,            // 골드 획득
    GemChanged,             // 보석 획득

    // 게임 상태
    GameStateChanged,       // 게임 오버, 일시정지 등
    BossAppeared            // 보스 등장
}

public class EventManager : BaseManager
{
    // 이벤트 저장소 (이벤트 타입, 실행할 함수들)
    // object 파라미터를 받는 Action을 저장
    private Dictionary<EEventType, Action<object>> _eventDictionary;

    protected override async UniTask OnInitialize()
    {
        await UniTask.CompletedTask;
    }

    // 1. 이벤트 듣기 시작 (구독)
    public void StartListening(EEventType eventType, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            // 이미 있으면 추가
            thisEvent += listener;
            _eventDictionary[eventType] = thisEvent;
        }
        else
        {
            // 없으면 새로 생성
            thisEvent += listener;
            _eventDictionary.Add(eventType, thisEvent);
        }
    }

    // 2. 이벤트 듣기 중단 (구독 취소) - ★ 메모리 누수 방지 위해 필수
    public void StopListening(EEventType eventType, Action<object> listener)
    {
        if (_eventDictionary == null) return;

        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventType] = thisEvent;
        }
    }

    // 3. 이벤트 발생 시키기 (방송)
    public void TriggerEvent(EEventType eventType, object param = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }
}
