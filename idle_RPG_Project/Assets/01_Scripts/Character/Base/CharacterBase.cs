using Cysharp.Threading.Tasks;
using DataTable;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

// 유닛의 상태 정의
public enum ECharacterState { Idle, Move, Attack, Dead }

[Serializable]
public class CharacterStat
{
    bool isAlive;

    public BigInteger MaxHp;
    public BigInteger CurrentHp;
    public BigInteger Atk;
    public BigInteger Def;

    public float CritRate;
    public float CritDmg;
    public float AtkRange;
    public float MoveSpd;
    public float AtkSpd;

    public CharacterStat(CharacterValue value)
    {
        isAlive = true;
        MaxHp = value.MaxHp;
        CurrentHp = MaxHp;
        Atk = value.Atk;
        Def = value.Def;
        CritRate = value.CritRate;
        CritDmg = value.CritDmg;
        AtkRange = value.AtkRange;
        MoveSpd = value.MoveSpd;
        AtkSpd = value.AtkSpd;
        
    }

    
}

public abstract class CharacterBase : MonoBehaviour
{
    [Header("캐릭터 상태")]
    [ReadOnly] public string ID;
    public ECharacterState State;
    public CharacterStat Stat;
    public BigInteger CombatPower => GetCombatPower(Stat);
    [SerializeField] public CharacterBase Target;


    protected HPBar _hpBar;
    protected virtual bool hpBarAlwaysVisible => false;

    protected Vector2 targetDir =>
        (Target == null) ? Vector2.right :
        (Target.transform.position - transform.position).normalized;

    [Title("공격/피격 관련")]
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] private int maxTargetCount = 3;
    [SerializeField] protected bool canKnockback = false;
    protected bool _isKnockbacked = false;
    private Tween _knockbackTween;
    protected Rigidbody2D _rigidbody;

    [Title("애니메이션")]
    [SerializeField] protected float baseMoveSpeed = 5.0f;
    [SerializeField] protected float attackAnimLength = 0.25f;

    protected Animator _animator;
    protected SpriteRenderer _spriteRenderer;
    protected float lastAttackTime;
    protected float _busyTimer = 0f;
    private Poolable _poolable;

    private float _lastHitEventTime = -1f;

    protected virtual void Awake()
    {
        // 컴포넌트 캐싱
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _poolable = GetComponent<Poolable>();
    }

    // --- 초기화 ---
    public virtual void Setup(CharacterValue value)
    {
        // 플레이어/몬스터 각자 초기화 로직 오버라이드
        Stat = new CharacterStat(value);
    }

    protected virtual void OnEnable()
    {
        _hpBar = MainSystem.FX.GetHpBar(transform, hpBarAlwaysVisible);
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

            _animator.SetFloat("DirX", targetDir.x);
            _animator.SetFloat("DirY", targetDir.y);

            // 3. 좌우 반전 (Flip) 처리
            // 오른쪽(X > 0)이면 Flip 끔, 왼쪽(X < 0)이면 Flip 켬
            if (targetDir.x > 0.01f)
                _spriteRenderer.flipX = false;
            else if (targetDir.x < -0.01f)
                _spriteRenderer.flipX = true;
        }
    }

    // --- 메인 로직 (BattleManager가 매 프레임 호출) ---
    public virtual void Tick(float deltaTime)
    {
        if (State == ECharacterState.Dead) return;
        if(_isKnockbacked) return;

        // 1. 행동 불가 상태인지 체크
        if (_busyTimer > 0)
        {
            _busyTimer -= deltaTime;

            // 공격 중에는 이동 애니메이션 파라미터를 끔
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", false);
            }
            return;
        }

        // 1. 타겟 유효성 검사 및 갱신
        CheckTarget();

        // 2. 거리 계산 및 행동 결정
        float distSqr = float.MaxValue;
        if (Target != null && Target.gameObject.activeSelf)
        {
            distSqr = (Target.transform.position - transform.position).sqrMagnitude;
        }

        float rangeSqr = Stat.AtkRange * Stat.AtkRange;

        float currentInterval = 1.0f / Mathf.Max(0.01f, Stat.AtkSpd); // 0 나누기 방지
        bool isCooldownReady = (Time.time - lastAttackTime >= currentInterval);

        // A. 사거리 안 -> 공격
        if (Target != null && distSqr <= rangeSqr)
        {
            if (isCooldownReady)
            {
                State = ECharacterState.Attack;
                UpdateAnimation(Vector2.zero); // 멈춤
                ProcessAttack();
                _busyTimer = attackAnimLength / Stat.AtkSpd;
            }
            else
            {
               State = ECharacterState.Idle;
                UpdateAnimation(Vector2.zero); // 멈춤
            }
        }
        // B. 사거리 밖 -> 이동 (타겟이 없으면 각자 방식대로 이동)
        else if(_busyTimer <=0)
        {
            State = ECharacterState.Move;
            MoveToTarget(deltaTime);
        }

        SyncAnimationSpeed();
    }

    // --- 행동 로직 (자식 클래스에서 오버라이드 가능) ---
    protected virtual void CheckTarget()
    {
        // 타겟이 죽거나 비활성화되면 null 처리
        if (Target != null && (Target.State == ECharacterState.Dead || !Target.gameObject.activeSelf))
        {
            Target = null;
        }

        // 타겟이 없을 때만 새로 찾기 시도 (Slayer는 가장 가까운 적, Monster는 Slayer)
        if (Target == null) FindTarget();
    }

    protected abstract void FindTarget(); // 아군/적군 검색 로직이 다르므로 abstract
    protected abstract void MoveToTarget(float deltaTime); // 이동 로직도 다르므로 abstract

    protected virtual void ProcessAttack()
    {
        lastAttackTime = Time.time;
        
        // 애니메이션 트리거 (이벤트로 데미지 줌)
        if (_animator != null) _animator.SetTrigger("Attack");
    }

    // 애니메이션 이벤트에서 호출될 함수
    public virtual void OnAttackEvent()
    {
        // 1. 마지막 공격 이벤트로부터 0.1초도 안 지났으면 무시 (버그 차단)
        // (프레임 단위 중복, 애니메이션 튀는 현상 방지)
        if (Time.time - _lastHitEventTime < 0.1f) return;

        _lastHitEventTime = Time.time; // 시간 갱신

        Vector2 origin = GetAttackOrigin();
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(origin, Stat.AtkRange * 0.5f, enemyLayer);

        // 중복 방지용 리스트
        HashSet<CharacterBase> uniqueEnemies = new HashSet<CharacterBase>();

        int count = 0;
        foreach (var hit in hitEnemies)
        {
            if (count >= maxTargetCount) break;

            CharacterBase enemy = hit.GetComponent<CharacterBase>();

            // 유효한 적이고, 아직 때린 목록에 없다면
            if (enemy != null && enemy.gameObject.activeSelf && !uniqueEnemies.Contains(enemy))
            {
                // 때린 목록에 추가
                uniqueEnemies.Add(enemy);

                // 데미지 적용. 추후 계산식 처리 후 실질적 데미지 적용
                enemy.TakeDamage(MakeAttackInfo());

                count++;
            }
        }
    }

    protected virtual AttackInfo MakeAttackInfo()
    {
        return new AttackInfo
        {
            Attacker = this,
            Damage = Stat.Atk,
            AttackType = EAttackType.Normal,
            Knockback = new KnockbackInfo
            {
                Distance = 0f,
                Duration = 0f
            }
        };
    }

    protected Vector2 GetAttackOrigin()
    {
        if (_spriteRenderer == null)
            return (Vector2)transform.position;
        return (Vector2)transform.position + targetDir * (Stat.AtkRange * 0.5f);
    }

    public virtual void TakeDamage(AttackInfo info)
    {
        if (State == ECharacterState.Dead) return;
        Stat.CurrentHp -= info.Damage;

        // (나중에 여기에 피격 이펙트/데미지 텍스트 추가)
        // 이펙트 적용
        MainSystem.FX.GetDamageText(transform.position, new AttackInfo
        {
            Attacker = this,
            Damage = info.Damage,
            AttackType = EAttackType.Normal
        });
        // HP바 갱신
        if (_hpBar != null)
        {
            _hpBar.SetHP(Stat.CurrentHp, Stat.MaxHp);
        }
        // 넉백 적용
        if (info.Knockback.Distance > 0)
        {
            Knockback(info.Attacker.transform.position, info.Knockback.Distance, info.Knockback.Duration);
        }

        if (Stat.CurrentHp <= 0)
        {
            Stat.CurrentHp = 0;
            Die();
        }
    }

    protected void Knockback(Vector3 attackerPos, float distance, float duration)
    {
        if (!canKnockback || State == ECharacterState.Dead) return;

        if (_knockbackTween != null && _knockbackTween.IsActive()) _knockbackTween.Kill();

        _isKnockbacked = true;
        if (_animator != null) _animator.SetTrigger("Hit");

        // 넉백 방향 및 목표 위치 계산
        Vector2 dir = (transform.position - attackerPos).normalized;
        if (dir == Vector2.zero) dir = Vector2.right; // 겹침 방지

        Vector2 targetPos = (Vector2)transform.position + (dir * distance);

        _knockbackTween = _rigidbody.DOMove(targetPos, duration)
            .SetEase(Ease.OutQuad)
            .SetUpdate(UpdateType.Fixed)
            .OnComplete(() =>
            {
                _isKnockbacked = false;
                // 죽지 않았으면 Idle로 복귀
                if (State != ECharacterState.Dead) State = ECharacterState.Idle;
            });
    }

    protected virtual void Die()
    {
        if (State == ECharacterState.Dead) return;
        State = ECharacterState.Dead;
        Target = null;
        gameObject.SetActive(false);

        // HP바 반납
        if (_hpBar != null)
        {
            MainSystem.FX.ReturnHpBar(_hpBar);
        }

        _poolable?.Release();

        // BattleManager에게 죽었다고 알리는 로직은 Event로 처리
        MainSystem.Battle.OnCharacterDead(this);
    }

    protected void SyncAnimationSpeed()
    {
        if (_animator == null) return;

        // 1. 이동 속도 (변화 없음)
        float moveMultiplier = Stat.MoveSpd / baseMoveSpeed;
        if (moveMultiplier < 0.1f) moveMultiplier = 1f;
        _animator.SetFloat("AnimMoveSpeed", moveMultiplier);

        // 2. ★ 공격 속도 배율 계산
        // 공식: 현재공속 * 애니메이션길이
        // 설명: 1초짜리 애니를 공속 2.0(0.5초 쿨)에 맞추려면 2배속으로 재생해야 함.
        //       (1.0 * 2.0 = 2.0배속)
        float attackMultiplier = Stat.AtkSpd * attackAnimLength;

        // 너무 느리게 재생되는 것 방지 (최소 1배속 유지 등 정책 결정 필요)
        if (attackMultiplier < 1.0f) attackMultiplier = 1.0f;

        _animator.SetFloat("AnimAttackSpeed", attackMultiplier);
    }

    public BigInteger GetCombatPower(CharacterStat unit)
    {
        BigInteger atkScore = unit.Atk * 15 / 10; // 1.5배
        BigInteger hpScore = unit.MaxHp * 5 / 10;    // 0.5배
        BigInteger defScore = unit.Def;           // 1.0배

        // 크리티컬 점수: (공격력 * 확률 * 데미지배율) 등을 반영하기도 함
        // 여기서는 단순 가산점으로 처리
        BigInteger critScore = (BigInteger)(unit.CritRate * unit.CritDmg);

        return atkScore + hpScore + defScore + critScore;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // 잘 보이게 빨간색으로 설정
        Gizmos.color = Color.red;

        // 위 함수를 통해 계산된 '진짜 공격 중심점' 가져오기
        Vector2 origin = GetAttackOrigin();

        // 그 위치에 공격 반경만큼 원 그리기
        Gizmos.DrawWireSphere(origin, Stat.AtkRange * 0.5f);
    }

}