using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Numerics;

public class PlayerManager : BaseManager
{
    [ShowInInspector]public PlayerData playerData { get; private set; }

    protected override async UniTask OnInitialize()
    {
        LoadData();
        await UniTask.CompletedTask;
    }
    private void LoadData()
    {
        TestLoadData();
    }

    private void TestLoadData()
    {
        // 데이터 로드 (임시로 새로 생성)
        playerData = new PlayerData();
        playerData.LoadData();
        Debug.Log("PlayerManager: Player data initialized.");
    }
}
