using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class LootManager : BaseManager
{
    [Title("Loot System")]
    [SerializeField] private List<LootItemDataSO> lootItemDataList; // 드랍 아이템 데이터 리스트
    private Dictionary<ELootType, LootItemDataSO> _lootDataDict = new();

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }

    private void Initialize()
    {
        // 드랍 아이템 데이터 딕셔너리 초기화
        foreach (var itemData in lootItemDataList)
        {
            if (!_lootDataDict.ContainsKey(itemData.type))
            {
                _lootDataDict.Add(itemData.type, itemData);
            }
        }
    }
    public void SpawnLoot(ELootType type, int amount, Vector3 spawnPosition, Transform target)
    {
        if (!_lootDataDict.ContainsKey(type))
        {
            Debug.LogError($"[LootManager] 데이터가 없습니다: {type}");
            return;
        }

        if (target == null) return;

        // 1. 풀에서 껍데기 가져오기
        LootItem item = MainSystem.Instance.FX.Pop(EPoolType.LootItem, spawnPosition).GetComponent<LootItem>();

        // 2. 데이터(SO) 주입해서 변신시키기
        item.Initialize(_lootDataDict[type], amount, spawnPosition, target);
    }
}
