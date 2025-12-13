using UnityEngine;
using System.Numerics;

public class AttackInfo
{
    public BigInteger Damage { get; set; }
    public EAttackType AttackType { get; set; }
    public bool IsMiss { get; set; } = false;
}
