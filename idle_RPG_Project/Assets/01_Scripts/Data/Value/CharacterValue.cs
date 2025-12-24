using Sirenix.OdinInspector;
using System.Numerics;
using UnityEngine;

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
    public virtual float CritRate => 0f;
    public virtual float CritDmg => 0f;
    public float attackRange;
    public float moveSpeed;
    public float attackSpeed; // 공격 속도 (초 당 공격 횟수)
}
