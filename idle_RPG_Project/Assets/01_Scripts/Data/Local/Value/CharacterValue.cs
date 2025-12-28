using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CharacterValue 
{
    public string ID;
    public string Name;
    public bool IsAlive;
    [ShowInInspector] public virtual BigInteger MaxHp { get; set; }
    [ShowInInspector] public BigInteger CurrentHp;
    [ShowInInspector] public virtual BigInteger Atk { get; set; }
    [ShowInInspector] public virtual BigInteger Def { get; set; }

    public BigInteger CombatPower => this.GetCombatPower();
    public float CritRate = 0f;
    public float CritDmg = 0f;
    public float AtkRange;
    public float MoveSpd;
    public float AtkSpd; // 공격 속도 (초 당 공격 횟수)
}
