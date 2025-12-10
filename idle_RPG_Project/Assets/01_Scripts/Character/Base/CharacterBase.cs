using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;
using UnityEngine.Timeline;
using Vector2 = UnityEngine.Vector2;

// 유닛의 상태 정의
public enum EntityState { Idle, Move, Attack, Dead }

public abstract class CharacterBase : MonoBehaviour
{
    [Title("스탯")]
    [ShowInInspector] public BigInteger MaxHp { get; protected set; }
    [ShowInInspector] public BigInteger CurrentHp { get; protected set; }
    [ShowInInspector] public BigInteger Atk { get; protected set; }
    [SerializeField] protected float attackRange = 2.0f;
    [SerializeField] protected float moveSpeed = 5.0f;
    [SerializeField] protected float attackSpeed = 1.0f; // 공격 속도 (초 당 공격 횟수)


    [Title("상태 & 비주얼")]
    [ShowInInspector, ReadOnly] public EntityState State { get; protected set; } = EntityState.Idle;

    [Title("공격 관련")]
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Vector2 attackOffset = new Vector2(1.0f, 0.5f); // 공격 중심점 오프셋
    [SerializeField] private float attackRadius = 1.5f; // 실제 타격 범위
    [SerializeField] private int maxTargetCount = 3;

    [Title("애니메이션")]
    [SerializeField] protected float baseMoveSpeed = 5.0f;
    [SerializeField] protected float attackAnimLength = 0.25f;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected CharacterBase target;
    protected float lastAttackTime;

    private void Awake()
    {
        // 컴포넌트 캐싱
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }

    // --- 초기화 ---
    public virtual void Setup(BigInteger hp, BigInteger atk)
    {
        MaxHp = hp;
        CurrentHp = hp;
        Atk = atk;
        State = EntityState.Idle;
        gameObject.SetActive(true);
    }

    // --- 애니메이션 통합 로직 ---
    protected void UpdateAnimation(Vector2 moveDir)
    {
        if (_animator == null) return;

        // 1. 이동 중인가? (벡터 길이가 0보다 크면 이동 중)
        bool isMoving = moveDir.sqrMagnitude > 0.01f;
        _animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            // 2. Blend Tree 파라미터 전달
            Vector2 normalizedDir = moveDir.normalized;

            _animator.SetFloat("DirX", normalizedDir.x);
            _animator.SetFloat("DirY", normalizedDir.y);

            // 3. 좌우 반전 (Flip) 처리
            // 오른쪽(X > 0)이면 Flip 끔, 왼쪽(X < 0)이면 Flip 켬
            if (normalizedDir.x > 0.01f)
                _spriteRenderer.flipX = false;
            else if (normalizedDir.x < -0.01f)
                _spriteRenderer.flipX = true;
        }
    }

    // --- 메인 로직 (BattleManager가 매 프레임 호출) ---
    public virtual void Tick(float deltaTime)
    {
        if (State == EntityState.Dead) return;

        // 1. 타겟 유효성 검사 및 갱신
        CheckTarget();

        // 2. 거리 계산 및 행동 결정
        float distSqr = float.MaxValue;
        if (target != null && target.gameObject.activeSelf)
        {
            distSqr = (target.transform.position - transform.position).sqrMagnitude;
        }

        float rangeSqr = attackRange * attackRange;

        float currentInterval = 1.0f / Mathf.Max(0.01f, attackSpeed); // 0 나누기 방지
        bool isCooldownReady = (Time.time - lastAttackTime >= currentInterval);

        // A. 사거리 안 -> 공격
        if (target != null && distSqr <= rangeSqr)
        {
            if (isCooldownReady)
            {
                State = EntityState.Attack;
                UpdateAnimation(Vector2.zero); // 멈춤
                ProcessAttack();
            }
            else
            {
                State = EntityState.Idle;
                UpdateAnimation(Vector2.zero); // 멈춤
            }
        }
        // B. 사거리 밖 -> 이동 (타겟이 없으면 각자 방식대로 이동)
        else
        {
            State = EntityState.Move;
            MoveToTarget(deltaTime);
        }

        SyncAnimationSpeed();
    }

    // --- 행동 로직 (자식 클래스에서 오버라이드 가능) ---
    protected virtual void CheckTarget()
    {
        // 타겟이 죽거나 비활성화되면 null 처리
        if (target != null && (target.State == EntityState.Dead || !target.gameObject.activeSelf))
        {
            target = null;
        }

        // 타겟이 없을 때만 새로 찾기 시도 (Slayer는 가장 가까운 적, Monster는 Slayer)
        if (target == null) FindTarget();
    }

    protected abstract void FindTarget(); // 아군/적군 검색 로직이 다르므로 abstract
    protected abstract void MoveToTarget(float deltaTime); // 이동 로직도 다르므로 abstract

    protected virtual void ProcessAttack()
    {
        lastAttackTime = Time.time;
        
        // 애니메이션 트리거 (이벤트로 데미지 줌)
        if (_animator != null) _animator.SetTrigger("Attack");
        Debug.Log($"{gameObject.name} attacks {target} with {Atk} damage");
    }

    // 애니메이션 이벤트에서 호출될 함수
    public virtual void OnAttackEvent()
    {
        Vector2 origin = GetAttackOrigin();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(origin, attackRadius, enemyLayer);

        int count = 0;
        foreach (var hit in hitEnemies)
        {
            if (count >= maxTargetCount) break;

            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy.gameObject.activeSelf)
            {
                enemy.TakeDamage(Atk);
                count++;
            }
        }
    }
    private Vector2 GetAttackOrigin()
    {
        float facingDir = _spriteRenderer.flipX ? -1 : 1;
        return (Vector2)transform.position + new Vector2(attackOffset.x * facingDir, attackOffset.y);
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

    protected void SyncAnimationSpeed()
    {
        if (_animator == null) return;

        // 1. 이동 속도 (변화 없음)
        float moveMultiplier = moveSpeed / baseMoveSpeed;
        if (moveMultiplier < 0.1f) moveMultiplier = 1f;
        _animator.SetFloat("AnimMoveSpeed", moveMultiplier);

        // 2. ★ 공격 속도 배율 계산
        // 공식: 현재공속 * 애니메이션길이
        // 설명: 1초짜리 애니를 공속 2.0(0.5초 쿨)에 맞추려면 2배속으로 재생해야 함.
        //       (1.0 * 2.0 = 2.0배속)
        float attackMultiplier = attackSpeed * attackAnimLength;

        // 너무 느리게 재생되는 것 방지 (최소 1배속 유지 등 정책 결정 필요)
        if (attackMultiplier < 1.0f) attackMultiplier = 1.0f;

        _animator.SetFloat("AnimAttackSpeed", attackMultiplier);
    }

    private void OnDrawGizmosSelected()
    {
        // 잘 보이게 빨간색으로 설정
        Gizmos.color = Color.red;

        // 위 함수를 통해 계산된 '진짜 공격 중심점' 가져오기
        Vector2 origin = GetAttackOrigin();

        // 그 위치에 공격 반경만큼 원 그리기
        Gizmos.DrawWireSphere(origin, attackRadius);
    }

}