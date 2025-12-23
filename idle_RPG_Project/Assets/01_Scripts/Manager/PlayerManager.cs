using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Numerics;

public class PlayerManager : BaseManager
{
    [ShowInInspector]public PlayerData playerData { get; private set; }


    private EventManager _eventManager;

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        // 플레이어 매니저 초기화 로직 작성
        if (playerData == null)
        {
            playerData = new PlayerData();
            Debug.Log("PlayerManager: PlayerData initialized.");
        }

        _eventManager = MainSystem.Instance.Event;
    }

    public void ChangeGold(object data)
    {
        if (data is BigInteger amount)
        {
            playerData.Gold += amount;
            _eventManager.TriggerEvent(EEventType.GoldChanged, playerData.Gold);
            Debug.Log($"PlayerManager: Added {amount} gold. Total Gold: {playerData.Gold}");
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
            playerData.Gem += amount;
            Debug.Log($"PlayerManager: Added {amount} gems. Total Gems: {playerData.Gem}");
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
            playerData.Experience += amount;
            Debug.Log($"PlayerManager: Gained {amount} experience. Total Experience: {playerData.Experience}");
            // 레벨업 로직 추가 가능
        }
        else
        {
            Debug.LogWarning("PlayerManager: Invalid data type for GainExperience.");
        }
    }
}
