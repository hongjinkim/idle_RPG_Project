using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;


public class Player: CharacterBase
{
    // 적에게 너무 딱 붙지 않게 하는 거리 (사거리의 80% 정도까지만 접근)
    private float StopDistance => attackRange * 0.8f;

    [Title("Combo Settings")]
    [SerializeField] protected float comboResetTime = 2.0f;
    protected int _comboIndex = 0; // 현재 콤보 순서 (0, 1, 2)

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

    // Player는 부모의 Tick을 쓰지 않고 자체 로직
    public override void Tick(float deltaTime)
    {
        if (State == EntityState.Dead) return;

        // [Lock 로직] (부모와 동일)
        if (_busyTimer > 0)
        {
            _busyTimer -= deltaTime;
            if (_animator != null) _animator.SetBool("IsMoving", false);
            return;
        }

        // 쿨타임 계산
        float cooldown = 1.0f / Mathf.Max(0.01f, attackSpeed);

        // 쿨타임이 찼고, 공격할 준비가 되었을 때
        if (Time.time - lastAttackTime >= cooldown)
        {
            // 주변에 적이 있는지 확인
            if (CheckEnemyInAttackRange())
            {
                // 마지막 공격으로부터 일정 시간이 지났으면 1타부터 다시 시작
                if (Time.time - lastAttackTime > comboResetTime)
                {
                    _comboIndex = 0;
                }

                // 상태 변경
                State = EntityState.Attack;
                UpdateAnimation(Vector3.zero);

                // 락 걸기
                _busyTimer = attackAnimLength / attackSpeed;

                // 공격 실행 (오버라이드한 함수 호출)
                ProcessAttack();
            }
            else
            {
                // 적 없으면 이동
                FindTarget();
                MoveToTarget(deltaTime);
            }
        }
        else
        {
            // 쿨타임 중 이동
            FindTarget();
            MoveToTarget(deltaTime);
        }

        SyncAnimationSpeed();
    }

    // ★ Player 전용 공격 로직 (3단 콤보)
    protected override void ProcessAttack()
    {
        lastAttackTime = Time.time;

        if (_animator != null)
        {
            // 1. 현재 콤보 번호 알려주기
            _animator.SetInteger("ComboIndex", _comboIndex);
            // 2. 공격 신호 보내기 (DoAttack)
            _animator.SetTrigger("Attack");

            // 3. 다음 콤보 준비 (0 -> 1 -> 2 -> 0)
            _comboIndex = (_comboIndex + 1) % 3;
        }
    }

    private bool CheckEnemyInAttackRange()
    {
        Vector2 origin = GetAttackOrigin();
        return Physics2D.OverlapCircle(origin, attackRange, enemyLayer);
    }
}
