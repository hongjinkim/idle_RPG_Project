using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class DataManager:BaseManager
{
    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        // 데이터 매니저 초기화 로직 작성
    }
}
