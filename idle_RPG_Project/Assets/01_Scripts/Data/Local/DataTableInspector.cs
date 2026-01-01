using Sirenix.OdinInspector;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class DataTableInspector : MonoBehaviour
{
    [TabGroup("Tabs", "GlobalConstant")][ShowInInspector] public Dictionary<string, DataTable.GlobalConstant> Constant = new Dictionary<string, DataTable.GlobalConstant>();
    [TabGroup("Tabs", "Stage")][ShowInInspector] public Dictionary<string, DataTable.Stage> Stage = new Dictionary<string, DataTable.Stage>();
    [TabGroup("Tabs", "Hero")][ShowInInspector] public Dictionary<string, DataTable.Hero> Hero = new Dictionary<string, DataTable.Hero>();
    [TabGroup("Tabs", "Enemy")][ShowInInspector] public Dictionary<string, DataTable.Enemy> Enemy = new Dictionary<string, DataTable.Enemy>();
    [TabGroup("Tabs", "EnemyGrowth")][ShowInInspector] public Dictionary<string, DataTable.EnemyGrowth> EnemyGrowth = new Dictionary<string, DataTable.EnemyGrowth>();
    [TabGroup("Tabs", "Upgrade")][ShowInInspector] public Dictionary<string, DataTable.Upgrade> Upgrade = new Dictionary<string, DataTable.Upgrade>();
    [TabGroup("Tabs", "Gear")][ShowInInspector] public Dictionary<string, DataTable.Gear> Gear = new Dictionary<string, DataTable.Gear>();
    [TabGroup("Tabs", "Shop")][ShowInInspector] public Dictionary<string, DataTable.Shop> Shop = new Dictionary<string, DataTable.Shop>();
    [TabGroup("Tabs", "Localization")][ShowInInspector] public Dictionary<string, DataTable.Localization> Localization = new Dictionary<string, DataTable.Localization>();

    [Button("Load DataTable", ButtonSizes.Large)]
    public void LoadData()
    {
        UnityGoogleSheet.LoadAllData();

        Constant = DataTable.GlobalConstant.GetDictionary();
        Stage = DataTable.Stage.GetDictionary();
        Hero = DataTable.Hero.GetDictionary();
        Enemy = DataTable.Enemy.GetDictionary();
        EnemyGrowth = DataTable.EnemyGrowth.GetDictionary();
        Upgrade = DataTable.Upgrade.GetDictionary();
        Gear = DataTable.Gear.GetDictionary();
        Shop = DataTable.Shop.GetDictionary();
        Localization = DataTable.Localization.GetDictionary();

        Debug.Log("데이터 테이블 불러오기 완료");
    }
}


