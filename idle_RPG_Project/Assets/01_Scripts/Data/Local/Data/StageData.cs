using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[Serializable]
public class StageData
{
    // 스테이지 데이터
    [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout, KeyLabel = "Stage ID", ValueLabel = "Info")]
    public Dictionary<string, StageValue> StageDict = new Dictionary<string, StageValue>();
    public List<StageValue> StageList = new List<StageValue>();

    private int stageCount;

    // 적 데이터
    [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout, KeyLabel = "Enemy ID", ValueLabel = "Info")]
    public Dictionary<string, EnemyValue> EnemyDict = new Dictionary<string, EnemyValue>();

    private int enemyCount;
    
    public void LoadData()
    {
        LoadEnemyData();
        LoadStageData();
    }
    private void LoadEnemyData()
    {
        var enemyList = DataTable.Enemy.GetList();
        enemyCount = enemyList.Count;

        for (int i = 0; i < enemyCount; i++)
        {
            var enemy = new EnemyValue();
            enemy.ID = enemyList[i].ID;
            enemy.Name = enemyList[i].Name;
            enemy.BaseHp = enemyList[i].Hp;
            enemy.BaseAtk = enemyList[i].Atk;
            enemy.BaseDef = enemyList[i].Def;
            enemy.BaseGold = enemyList[i].GoldDrop;
            enemy.AtkRange = enemyList[i].AtkRange;
            enemy.MoveSpd = enemyList[i].MoveSpd;
            enemy.AtkSpd = enemyList[i].AtkSpd;

            EnemyDict[enemy.ID] = enemy;
        }
    }
    private void LoadStageData()
    {
        StageList.Clear();
        var stageList = DataTable.Stage.GetList();
        
        stageCount = stageList.Count;

        for (int i = 0; i < stageCount; i++)
        {
            var stage = new StageValue();

            stage.StageNum = i+1;
            stage.ID = stageList[i].ID;
            stage.Name = stageList[i].Name;
            foreach (var id in stageList[i].EnemyID)
            {
                stage.EnemyList.Add(GetEnemyValueInStage(id, i));
            }
            stage.EnemyBoss = GetEnemyValueInStage(stageList[i].BossID, i, true);


            StageDict[stage.ID] = stage;
            StageList.Add(stage);
        }
    }
    


    private EnemyValue GetEnemyValueInStage(string id, int stage, bool isBoss = false)
    {
        var enemy = GetEnemyValue(id);

        if (enemy == null)
            return null;

        var EnemyGrowthDict = DataTable.EnemyGrowth.GetDictionary();
        var newEnemy = new EnemyValue();

        newEnemy.ID = enemy.ID;
        newEnemy.Name = enemy.Name;
        newEnemy.AtkRange = enemy.AtkRange;
        newEnemy.MoveSpd = enemy.MoveSpd;
        newEnemy.AtkSpd = enemy.AtkSpd;

        // 스테이지에 맞게 성장한 스탯
        // 체력
        newEnemy.EnemyHP.Start = enemy.BaseHp;
        newEnemy.EnemyHP.Constant = EnemyGrowthDict["Constant"].Hp;
        newEnemy.EnemyHP.Exponent = (double)EnemyGrowthDict["Exponent"].Hp;
        newEnemy.EnemyHP.SetEnemyStat(stage);

        //공격력
        newEnemy.EnemyAttack.Start = enemy.BaseAtk;
        newEnemy.EnemyAttack.Constant = EnemyGrowthDict["Constant"].Atk;
        newEnemy.EnemyAttack.Exponent = (double)EnemyGrowthDict["Exponent"].Atk;
        newEnemy.EnemyAttack.SetEnemyStat(stage);

        //방어력
        newEnemy.EnemyDefence.Start = enemy.BaseDef;
        newEnemy.EnemyDefence.Constant = EnemyGrowthDict["Constant"].Def;
        newEnemy.EnemyDefence.Exponent = (double)EnemyGrowthDict["Exponent"].Def;
        newEnemy.EnemyDefence.SetEnemyStat(stage);

        //골드
        newEnemy.EnemyGold.Start = enemy.BaseGold;
        newEnemy.EnemyGold.Constant = EnemyGrowthDict["Constant"].GoldDrop;
        newEnemy.EnemyGold.Exponent = (double)EnemyGrowthDict["Exponent"].GoldDrop;
        newEnemy.EnemyGold.SetEnemyStat(stage);

        if (isBoss)
        {
            //보스 체력 배수
            newEnemy.BossHPMultiplier.Start = EnemyGrowthDict["Start"].Boss_HPMultiply;
            newEnemy.BossHPMultiplier.Constant = EnemyGrowthDict["Constant"].Boss_HPMultiply;
            newEnemy.BossHPMultiplier.Exponent = (double)EnemyGrowthDict["Exponent"].Boss_HPMultiply;
            newEnemy.BossHPMultiplier.SetEnemyStat(stage);

            //보스 공격력 배수
            newEnemy.BossAttackMultiplier.Start = EnemyGrowthDict["Start"].Boss_AttackMultiply;
            newEnemy.BossAttackMultiplier.Constant = EnemyGrowthDict["Constant"].Boss_AttackMultiply;
            newEnemy.BossAttackMultiplier.Exponent = (double)EnemyGrowthDict["Exponent"].Boss_AttackMultiply;
            newEnemy.BossAttackMultiplier.SetEnemyStat(stage);
        }


        return newEnemy;
    }

    public StageValue GetStageValue(string id)
    {
        return StageDict.TryGetValue(id, out var stage) ? stage : null;
    }
    public StageValue GetStageValue(int stageNum)
    {
        if(stageNum < 0 || stageNum >= StageList.Count)
            return null;
        return StageList[stageNum-1];
    }
    public EnemyValue GetEnemyValue(string id)
    {
        return EnemyDict.TryGetValue(id, out var enemy) ? enemy : null;
    }

}
