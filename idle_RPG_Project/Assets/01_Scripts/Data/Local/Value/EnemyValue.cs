using System.Collections;
using UnityEngine;
using System;
using System.Numerics;

[System.Serializable]
public class EnemyValue : CharacterValue
{
    public EnemyStatValue EnemyHP = new EnemyStatValue();
    public EnemyStatValue EnemyAttack = new EnemyStatValue();
    public EnemyStatValue EnemyDefence = new EnemyStatValue();
    public EnemyStatValue EnemyGold = new EnemyStatValue();
    public DoubleEnemyStatValue BossAttackMultiplier = new DoubleEnemyStatValue();
    public DoubleEnemyStatValue BossHPMultiplier = new DoubleEnemyStatValue();

    public BigInteger BaseHp;
    public BigInteger BaseAtk;
    public BigInteger BaseDef;
    public BigInteger BaseGold;


    public override BigInteger MaxHp => EnemyHP.Stat;
    public override BigInteger Atk => EnemyAttack.Stat;
    public override BigInteger Def => EnemyDefence.Stat;
    public BigInteger GoldDrop => EnemyGold.Stat;
}
[Serializable]
public class EnemyStatValue
{
    public BigInteger Start;
    public BigInteger Constant; // 상수곱
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
    public BigInteger Start;
    public BigInteger Constant;   // 상수곱
    public double Exponent;
    public double Stat;

    public void SetEnemyStat(int stage)
    {
        int s = Math.Max(stage, 1);

        Stat = (double)Start.ExpGrowth(Constant, Exponent, s);
    }
}
