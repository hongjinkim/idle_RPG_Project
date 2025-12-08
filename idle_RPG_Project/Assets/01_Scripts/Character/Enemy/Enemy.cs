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
    protected override void MoveToTarget(float deltaTime)
    {
        transform.Translate(Vector2.left * moveSpeed * deltaTime);
    }
}
