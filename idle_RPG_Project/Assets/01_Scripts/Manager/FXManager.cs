using UnityEngine;
using UnityEngine.Pool;
using Cysharp.Threading.Tasks;

public class FXManager : PoolManager<FXManager, EPoolType>
{
    private Camera effectCamera;
    protected override async UniTask OnInitialize()
    {
        await base.OnInitialize();
        effectCamera = Camera.main;
    }

    public GameObject DamageTextEffect(Vector2 position, AttackInfo attackInfo)
    {
        var obj = Pop(EPoolType.DamageText, position);
        obj.GetComponentInChildren<DamageTextEffect>().SetDamage(attackInfo);
        return obj;
    }
}
