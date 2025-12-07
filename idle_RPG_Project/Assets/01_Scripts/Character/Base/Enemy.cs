using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Enemy: CharacterBase
{
    protected override void FindTarget()
    {
        // 몬스터의 타겟은 기본적으로 슬레이어
        target = MainSystem.Instance.Battle.Player;
    }

    // 몬스터는 슬레이어가 없어도 왼쪽(또는 특정 방향)으로 계속 갈 수도 있음
    protected override void MoveToTarget(float deltaTime)
    {
        if (target != null)
        {
            base.MoveToTarget(deltaTime);
        }
        else
        {
            // 타겟 없으면 그냥 앞으로 전진? (디펜스 게임 룰에 따라 결정)
            transform.position += Vector3.left * moveSpeed * deltaTime;
        }
    }
}
