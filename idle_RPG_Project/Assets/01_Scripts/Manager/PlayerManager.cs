using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class PlayerManager : BaseManager
{
    [ShowInInspector]public PlayerData Data { get; private set; }

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
    }
}
