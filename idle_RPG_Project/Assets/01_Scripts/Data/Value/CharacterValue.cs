using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CharacterValue 
{
    public int Id;
    public string Name;
    public bool IsAlive;
    [ShowInInspector] public BigInteger MaxHp;
    [ShowInInspector] public BigInteger CurrentHp;
    [ShowInInspector] public BigInteger Atk;
    [ShowInInspector] public BigInteger Def;

    public BigInteger CombatPower => this.GetCombatPower();
    public float CritRate = 0f;
    public float CritDmg = 0f;
    public float AtkRange;
    public float MoveSpd;
    public float AtkSpd; // 공격 속도 (초 당 공격 횟수)
}
