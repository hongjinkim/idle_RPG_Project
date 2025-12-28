using System.Collections.Generic;

[System.Serializable]
public class StageValue
{
    public int StageNum;
    public string ID;
    public string Name;

    public List<EnemyValue> EnemyList = new List<EnemyValue>();
    public EnemyValue EnemyBoss;
}