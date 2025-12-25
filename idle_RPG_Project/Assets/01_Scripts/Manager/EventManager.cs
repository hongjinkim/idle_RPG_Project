using UnityEngine;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public enum EEventType
{
    // 플레이어 관련
    PlayerExpChanged,       // 경험치 변경
    PlayerLevelUp,          // 레벨
    PlayerPowerUpdated,     // 전투력 변경

    // 재화 관련
    GoldUpdated,            // 골드 변경
    GemUpdated,             // 보석 변경

    // 게임 상태
    GameStateChanged,       // 게임 오버, 일시정지 등
    BossAppeared            // 보스 등장
}
public class EventManager : BaseManager
{
    private Dictionary<EEventType, Action<object>> _eventDictionary = new Dictionary<EEventType, Action<object>>();

    protected override async UniTask OnInitialize()
    {
        await UniTask.CompletedTask;
    }
    // ... StartListening, StopListening, TriggerEvent 기존 코드는 그대로 유지 ...
    public void StartListening(EEventType eventType, Action<object> listener)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent += listener;
            _eventDictionary[eventType] = thisEvent;
        }
        else
        {
            thisEvent += listener;
            _eventDictionary.Add(eventType, thisEvent);
        }
    }

    public void StopListening(EEventType eventType, Action<object> listener)
    {
        if (_eventDictionary == null) return;
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent -= listener;
            _eventDictionary[eventType] = thisEvent;
        }
    }

    public void TriggerEvent(EEventType eventType, object param = null)
    {
        if (_eventDictionary.TryGetValue(eventType, out Action<object> thisEvent))
        {
            thisEvent?.Invoke(param);
        }
    }

    
}