using UnityEngine;

public class HeroValue : CharacterValue
{
    // 영웅만의 고유 필드
    private float _critRate;
    private float _critDamage;

    // 부모의 가상 프로퍼티를 오버라이드해서 내 값을 반환
    public override float CritRate => _critRate;
    public override float CritDmg => _critDamage;
}
