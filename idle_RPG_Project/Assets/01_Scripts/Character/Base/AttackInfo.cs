using UnityEngine;
using System.Numerics;

[System.Serializable]
public struct knockbackInfo
{
    public float Distance { get; set; }
    public float Duration { get; set; }
}
public class AttackInfo
{
    public CharacterBase Attacker { get; set; }
    public BigInteger Damage { get; set; }
    public EAttackType AttackType { get; set; }
    public bool IsMiss { get; set; } = false;
    public knockbackInfo Knockback { get; set; } = new knockbackInfo { Distance = 0f, Duration = 0f };
}
