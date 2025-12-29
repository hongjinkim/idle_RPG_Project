using System.Collections;
using UnityEngine;
using System;
using System.Numerics;

[System.Serializable]
public class EnemyValue : CharacterValue
{
    [HideInInspector] public EnemyStatValue EnemyHP = new EnemyStatValue();
    [HideInInspector] public EnemyStatValue EnemyAttack = new EnemyStatValue();
    [HideInInspector] public EnemyStatValue EnemyDefence = new EnemyStatValue();
    [HideInInspector] public EnemyStatValue EnemyGold = new EnemyStatValue();
    [HideInInspector] public DoubleEnemyStatValue BossAttackMultiplier = new DoubleEnemyStatValue();
    [HideInInspector] public DoubleEnemyStatValue BossHPMultiplier = new DoubleEnemyStatValue();

    [HideInInspector] public BigInteger BaseHp;
    [HideInInspector] public BigInteger BaseAtk;
    [HideInInspector] public BigInteger BaseDef;
    [HideInInspector] public BigInteger BaseGold;


    public override BigInteger MaxHp => EnemyHP.Stat;
    public override BigInteger Atk => EnemyAttack.Stat;
    public override BigInteger Def => EnemyDefence.Stat;
    public BigInteger GoldDrop => EnemyGold.Stat;
}
[Serializable]
public class EnemyStatValue
{
    public BigInteger Start;
    public double Constant; // 상수곱
    public double Exponent;
    public BigInteger Stat;


    public void SetEnemyStat(int stage)
    {
        int s = Math.Max(stage, 1); // 1부터 설계 가정
        Stat = Start.ExpGrowth(Constant, Exponent, s);
    }
}
[Serializable]
public class DoubleEnemyStatValue
{
    public double Start;
    public double Constant;   // 상수곱
    public double Exponent;
    public double Stat;

    public void SetEnemyStat(int stage)
    {
        int s = Math.Max(stage, 1);
        Stat = Start.ExpGrowth(Constant, Exponent, s);
    }
}
