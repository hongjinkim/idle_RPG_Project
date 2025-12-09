using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class Enemy: CharacterBase
{
    [Title("AI 세팅")]
    [SerializeField] private float detectRange = 8.0f; // 플레이어 감지 거리
    [SerializeField] private float wanderRadius = 3.0f; // 배회 반경
    [SerializeField] private float wanderWaitTime = 2.0f; // 이동 후 대기 시간
    [SerializeField] private float wanderMoveSpeed = 1.5f; // 배회 시 이동 속도 (추격보다 느리게)

    // 내부 변수
    private Vector3 _wanderTarget;
    private float _waitTimer;
    private bool _isWaiting = false;

    // 초기화 시 배회 타이머 리셋
    public override void Setup(System.Numerics.BigInteger hp, System.Numerics.BigInteger atk)
    {
        base.Setup(hp, atk);
        _waitTimer = 0;
        SetNewWanderTarget(); // 태어나자마자 갈 곳 정하기
    }


    protected override void FindTarget()
    {
        Player player = MainSystem.Instance.Battle.Player;
        if (player == null || !player.gameObject.activeSelf) return;

        // 플레이어가 감지 범위 안에 들어왔는지 체크
        float distSqr = (player.transform.position - transform.position).sqrMagnitude;
        if (distSqr <= detectRange * detectRange)
        {
            target = player; // 발견! 즉시 타겟 설정
        }
    }
    // Tick 오버라이드: 상태 결정 로직 (추격 vs 배회)
    public override void Tick(float deltaTime)
    {
        if (State == EntityState.Dead) return;

        CheckTarget(); // 타겟 감지 시도

        // 1. 타겟(플레이어)이 있을 때 -> 기존 추격/공격 로직
        if (target != null)
        {
            base.Tick(deltaTime); // 부모의 기본 로직(공격/이동) 사용
        }
        // 2. 타겟이 없을 때 -> 배회(Wander) 로직
        else
        {
            WanderBehavior(deltaTime);
        }
    }

    // 배회 전용 로직
    private void WanderBehavior(float deltaTime)
    {
        // 대기 중이라면 시간만 깎음
        if (_isWaiting)
        {
            State = EntityState.Idle;
            UpdateAnimation(Vector3.zero); // 멈춤 애니메이션

            _waitTimer -= deltaTime;
            if (_waitTimer <= 0)
            {
                _isWaiting = false;
                SetNewWanderTarget(); // 대기 끝, 새로운 목표지점 설정
            }
            return;
        }

        // 이동 중
        State = EntityState.Move;

        // 목표 지점까지 거리 계산
        Vector3 diff = _wanderTarget - transform.position;

        // 도착 했는가? (아주 가까우면)
        if (diff.sqrMagnitude < 0.1f)
        {
            // 도착! 대기 모드 전환
            _isWaiting = true;
            _waitTimer = wanderWaitTime;
            UpdateAnimation(Vector3.zero);
        }
        else
        {
            // 이동
            Vector3 moveDir = diff.normalized;
            transform.Translate(moveDir * wanderMoveSpeed * deltaTime);

            // 배회 애니메이션 (느리게 걷기)
            UpdateAnimation(moveDir);
        }
    }

    // 현재 위치 주변 랜덤 좌표 생성
    private void SetNewWanderTarget()
    {
        // UnitCircle: 반지름 1인 원 내부의 랜덤 좌표 (2D)
        Vector2 randomPoint = Random.insideUnitCircle * wanderRadius;

        // 현재 위치 + 랜덤 좌표
        _wanderTarget = transform.position + new Vector3(randomPoint.x, randomPoint.y, 0);
    }

    // MoveForward는 "타겟이 있을 때(추격)" 사용됨
    protected override void MoveToTarget(float deltaTime)
    {
        Vector3 moveDir = Vector3.zero;

        if (target != null)
        {
            Vector3 diff = target.transform.position - transform.position;

            // 공격 사거리보다 멀면 추격
            if (diff.sqrMagnitude > attackRange * attackRange)
            {
                moveDir = diff.normalized;
                transform.Translate(moveDir * moveSpeed * deltaTime); // 여기선 원래 속도(빠름) 사용
            }
        }

        UpdateAnimation(moveDir);
    }

    public override void TakeDamage(System.Numerics.BigInteger damage)
    {
        base.TakeDamage(damage);

        // 맞으면 배회 멈추고 즉시 플레이어 쳐다봄
        if (target == null && CurrentHp > 0)
        {
            target = MainSystem.Instance.Battle.Player;
            _isWaiting = false; // 대기 취소
        }
    }

    private void OnDrawGizmosSelected()
    {
        // 감지 범위 (노랑)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // 배회 목표지점 (초록 선) - 디버깅용
        if (target == null && !_isWaiting)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, _wanderTarget);
            Gizmos.DrawWireSphere(_wanderTarget, 0.2f);
        }
    }
}
