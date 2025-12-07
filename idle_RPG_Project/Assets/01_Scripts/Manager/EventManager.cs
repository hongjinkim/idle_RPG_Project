using UnityEngine;
using Cysharp.Threading.Tasks;

public class EventManager : BaseManager
{
    protected override async UniTask OnInitialize()
    {
        await UniTask.Delay(500);
    }
}
