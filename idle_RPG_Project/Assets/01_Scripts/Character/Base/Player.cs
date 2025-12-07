using UnityEngine;
using System.Collections;


public class Player: CharacterBase
{
    protected override void FindTarget()
    {
        // BattleManager에게 가장 가까운 몬스터를 찾아달라고 요청
        target = MainSystem.Instance.Battle.GetNearestEnemy(transform.position);
    }
}
