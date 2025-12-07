using UnityEngine;
using System.Numerics;
using Sirenix.OdinInspector;

// 유닛의 상태 정의
public enum EntityState { Idle, Move, Attack, Dead }

public abstract class CharacterBase : MonoBehaviour
{
    // --- 스탯 (Odin으로 보기 편하게) ---
    [Title("Stats")]
    [ShowInInspector] public BigInteger MaxHp { get; protected set; }
    [ShowInInspector] public BigInteger CurrentHp { get; protected set; }
    [ShowInInspector] public BigInteger Atk { get; protected set; }

    [SerializeField] protected float attackRange = 2.0f;
    [SerializeField] protected float moveSpeed = 5.0f;
    [SerializeField] protected float attackInterval = 1.0f; // 공격 속도 (초)

    // --- 상태 변수 ---
    [Title("State")]
    [ShowInInspector, ReadOnly] public EntityState State { get; protected set; } = EntityState.Idle;
    protected CharacterBase target;
    protected float lastAttackTime;

    // --- 초기화 ---
    public virtual void Setup(BigInteger hp, BigInteger atk)
    {
        MaxHp = hp;
        CurrentHp = hp;
        Atk = atk;
        State = EntityState.Idle;
        gameObject.SetActive(true);
    }

    // --- 메인 로직 (BattleManager가 매 프레임 호출) ---
    public virtual void Tick(float deltaTime)
    {
        if (State == EntityState.Dead) return;

        // 1. 타겟이 없거나 죽었으면 새로 찾기
        if (target == null || target.State == EntityState.Dead || !target.gameObject.activeSelf)
        {
            FindTarget();
            if (target == null)
            {
                State = EntityState.Idle; // 적이 없으면 대기 (혹은 이동)
                return;
            }
        }

        // 2. 거리 체크
        float distSqr = (target.transform.position - transform.position).sqrMagnitude;
        float rangeSqr = attackRange * attackRange;

        if (distSqr <= rangeSqr)
        {
            // 공격 사거리 안
            State = EntityState.Attack;
            ProcessAttack();
        }
        else
        {
            // 사거리 밖 -> 이동
            State = EntityState.Move;
            MoveToTarget(deltaTime);
        }
    }

    // --- 행동 로직 (자식 클래스에서 오버라이드 가능) ---
    protected abstract void FindTarget(); // 아군/적군 검색 로직이 다르므로 abstract

    protected virtual void MoveToTarget(float deltaTime)
    {
        if (target == null) return;

        // 타겟 방향으로 이동
        UnityEngine.Vector3 dir = (target.transform.position - transform.position).normalized;
        transform.position += (dir * moveSpeed * deltaTime);
    }

    protected virtual void ProcessAttack()
    {
        // 쿨타임 체크
        if (Time.time - lastAttackTime < attackInterval) return;

        lastAttackTime = Time.time;

        // 실제 데미지 전달
        target.TakeDamage(Atk);

        // (나중에 여기에 공격 애니메이션 재생 코드 추가)
        Debug.Log($"{name} attacks {target.name} for {Atk.ToFormattedString(ENumberFormatType.Standard)} Damage!");
    }

    public virtual void TakeDamage(BigInteger damage)
    {
        CurrentHp -= damage;

        // (나중에 여기에 피격 이펙트/데미지 텍스트 추가)

        if (CurrentHp <= 0)
        {
            CurrentHp = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        State = EntityState.Dead;
        gameObject.SetActive(false);

        // BattleManager에게 죽었다고 알리는 로직은 Event로 처리
        MainSystem.Instance.Battle.OnCharacterDead(this);
    }
}