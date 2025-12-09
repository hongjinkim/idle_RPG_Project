using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


public class Player: CharacterBase
{
    // 적에게 너무 딱 붙지 않게 하는 거리 (사거리의 80% 정도까지만 접근)
    private float StopDistance => attackRange * 0.8f;


    private void Start()
    {
        Setup(1000, 50);
    }

    protected override void FindTarget()
    {
        // BattleManager에게 가장 가까운 몬스터를 찾아달라고 요청
        target = MainSystem.Instance.Battle.GetNearestEnemy(transform.position);
    }

    protected override void MoveToTarget(float deltaTime)
    {
        Vector2 moveDir = Vector2.zero;

        if (target != null)
        {
            // 1. 타겟 방향 계산 (Target - Me)
            Vector2 diff = target.transform.position - transform.position;
            float distSqr = diff.sqrMagnitude;

            // 2. 너무 멀면 접근, 공격 사거리 안쪽이면 멈춤 (카이팅 방지 및 제자리 공격)
            if (distSqr > StopDistance * StopDistance)
            {
                moveDir = diff.normalized;
                transform.Translate(moveDir * moveSpeed * deltaTime);
            }
            else
            {
                // 사거리 안에 들어왔으니 이동 멈춤 -> 공격 준비
                // (Tick에서 사거리 체크 후 공격 상태로 넘어감)
                moveDir = Vector3.zero;
            }
        }
        else
        {
            // 타겟(적)이 아예 없으면? -> 제자리 대기 (Idle)
            moveDir = Vector3.zero;
            State = EntityState.Idle;
        }

        // 애니메이션 갱신
        UpdateAnimation(moveDir);
    }
}
