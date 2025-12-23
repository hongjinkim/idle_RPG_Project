using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class PlayerManager : BaseManager
{
    [ShowInInspector]public PlayerData Data { get; private set; }


    private EventManager _eventManager;

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        // 플레이어 매니저 초기화 로직 작성
        if (Data == null)
        {
            Data = new PlayerData();
            Debug.Log("PlayerManager: PlayerData initialized.");
        }

        _eventManager = MainSystem.Instance.Event;
        SubsribeEvents();
    }

    private void SubsribeEvents()
    {
        _eventManager.StartListening(EEventType.GoldChanged, ChangeGold);
        _eventManager.StartListening(EEventType.GemChanged, ChangeGem);
        _eventManager.StartListening(EEventType.PlayerExpChanged, GainExperience);
    }

    public void ChangeGold(object data)
    {
        if (data is int amount)
        {
            Data.Gold += amount;
            Debug.Log($"PlayerManager: Added {amount} gold. Total Gold: {Data.Gold}");
        }
        else
        {
            Debug.LogWarning("PlayerManager: Invalid data type for AddGold.");
        }
    }
    public void ChangeGem(object data)
    {
        if (data is int amount)
        {
            Data.Gem += amount;
            Debug.Log($"PlayerManager: Added {amount} gems. Total Gems: {Data.Gem}");
        }
        else
        {
            Debug.LogWarning("PlayerManager: Invalid data type for AddGem.");
        }
    }
    public void GainExperience(object data)
    {
        if (data is int amount)
        {
            Data.Experience += amount;
            Debug.Log($"PlayerManager: Gained {amount} experience. Total Experience: {Data.Experience}");
            // 레벨업 로직 추가 가능
        }
        else
        {
            Debug.LogWarning("PlayerManager: Invalid data type for GainExperience.");
        }
    }
}
