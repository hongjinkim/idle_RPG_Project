using UnityEngine;
using UnityEngine.Pool;

public class FXManager : PoolManager<FXManager, EPoolType>
{
    public GameObject DamageTextEffect(Vector2 position, AttackInfo attackInfo)
    {
        var obj = Pop(EPoolType.DamageText, position);
        obj.GetComponentInChildren<DamageTextEffect>().SetDamage(attackInfo);
        return obj;
    }
}
