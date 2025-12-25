using System.Numerics;
using UnityEngine;

public static class CharacterValueExtension
{
    // ========================================================================
    // ⚔️ 1. 전투력 (Combat Power) 계산
    // ========================================================================
    /// <summary>
    /// 유닛의 종합 전투력을 계산해서 반환합니다. (보여주기용)
    /// 공식 예시: 공격력 * 1.5 + 체력 * 0.5 + 방어력 * 1.0 + (크리티컬 보정)
    /// </summary>
    public static BigInteger GetCombatPower(this CharacterValue unit)
    {
        BigInteger atkScore = unit.Atk * 15 / 10; // 1.5배
        BigInteger hpScore = unit.MaxHp * 5 / 10;    // 0.5배
        BigInteger defScore = unit.Def;           // 1.0배

        // 크리티컬 점수: (공격력 * 확률 * 데미지배율) 등을 반영하기도 함
        // 여기서는 단순 가산점으로 처리
        BigInteger critScore = (BigInteger)(unit.CritRate * unit.CritDmg);

        return atkScore + hpScore + defScore + critScore;
    }

    // ========================================================================
    // 💥 2. 실시간 데미지 계산 (공격 시 호출)
    // ========================================================================
    /// <summary>
    /// 상대방(target)을 공격했을 때 입힐 최종 데미지를 계산합니다. (크리티컬 포함)
    /// </summary>
    /// <param name="attacker">공격자</param>
    /// <param name="target">방어자</param>
    /// <param name="isCritical">크리티컬 발생 여부(out)</param>
    /// <returns>최종 데미지</returns>
    public static AttackInfo CalculateAttack(this CharacterBase attacker, CharacterBase target, float skillMult = 1.0f, KnockbackInfo? knockback = null)
    {
        // 1. 결과 객체 생성
        AttackInfo info = new AttackInfo
        {
            Attacker = attacker,
            Target = target,
            Knockback = knockback ?? new KnockbackInfo(), // null이면 기본값
            AttackType = EAttackType.Normal,
            IsMiss = false,
            Damage = 0
        };

        // 데이터 편의 참조 (CharacterBase에 Data 프로퍼티가 있다고 가정)
        var attackerValue = attacker.Stat.Value;
        var targetValue = target.Stat.Value;

        // ================================================================
        // 2. 회피(Miss) 판정
        // ================================================================
        // 예: (명중률 - 회피율) 공식 등을 사용
        // 여기선 단순하게 타겟의 회피율(Evasion)만 체크한다고 가정
        // float hitChance = 100f - defStat.EvasionRate; 
        // if (Random.Range(0f, 100f) > hitChance) 
        // {
        //     info.IsMiss = true;
        //     return info; // 데미지 0으로 리턴
        // }

        // ================================================================
        // 3. 크리티컬 판정
        // ================================================================
        bool isCrit = Random.Range(0f, 100f) < attackerValue.CritRate;
        if (isCrit)
        {
            info.AttackType = EAttackType.Critical;
        }

        // ================================================================
        // 4. 데미지 계산
        // ================================================================

        // (1) 기본 데미지: (공격력 - 방어력) * 스킬계수
        BigInteger baseDmg = attackerValue.Atk - targetValue.Def;
        if (baseDmg < 1) baseDmg = 1; // 최소 데미지 보정

        // 스킬 계수 적용 (float 연산 후 BigInteger 변환)
        // 정밀도를 위해 100을 곱해서 나누는 방식 추천
        baseDmg = baseDmg * (BigInteger)(skillMult * 100) / 100;

        // (2) 크리티컬 적용
        if (isCrit)
        {
            // 크리티컬 데미지% 적용 (예: 150% -> 1.5배)
            baseDmg = baseDmg * (BigInteger)attackerValue.CritDmg / 100;
        }

        // (3) 최종값 할당
        info.Damage = baseDmg;

        return info;
    }

    // ========================================================================
    // 🎲 3. 유틸리티 (확률 체크 등)
    // ========================================================================
    /// <summary>
    /// 크리티컬 발생 여부 체크
    /// </summary>
    public static bool CheckCritical(float critRate)
    {
        // Random.value는 0.0 ~ 1.0 반환 -> 100을 곱해서 0 ~ 100% 비교
        return (Random.value * 100f) <= critRate;
    }
}
