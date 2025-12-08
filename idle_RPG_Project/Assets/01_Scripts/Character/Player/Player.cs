using UnityEngine;
using System.Collections;


public class Player: CharacterBase
{
    private void Start()
    {
        Setup(1000, 50);
    }
    protected override void FindTarget()
    {
        // BattleManager에게 가장 가까운 몬스터를 찾아달라고 요청
        target = MainSystem.Instance.Battle.GetNearestEnemy(transform.position);
    }
    protected override void MoveForward(float deltaTime)
    {
        // 타겟이 있든 없든 사거리 밖이면 무조건 오른쪽으로 전진
        transform.Translate(Vector2.right * moveSpeed * deltaTime);
    }
}
