using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using System.Numerics;
using System;
using UGS;

[Serializable]
public class Constant
{
    public BigInteger InitGold;
    public float InitCritRate;
    public float InitCritDamage;

    public void LoadData()
    {
        var constant = DataTable.GlobalConstant.GetList()[0];
        InitGold = constant.InitGold;
        InitCritRate = constant.InitCritRate;
        InitCritDamage = constant.InitCritDamage;
    }
}

[RequireComponent(typeof(DataTableInspector))]
public class DataManager:BaseManager
{
    private DataTableInspector dataTable;
    public bool IsLoaded { get; private set; }
    public Constant Constants { get; private set; }

    [TabGroup("Tabs", "Constant"), HideLabel][InlineProperty] public Constant Constant = new Constant();
    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty] public HeroData Hero = new HeroData();
    //[TabGroup("Tabs", "Enemy"), HideLabel][InlineProperty] public EnemyData Enemy = new EnemyData();
    [TabGroup("Tabs", "Stage"), HideLabel][InlineProperty] public StageData Stage = new StageData();

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        LoadData();
    }


    [Button("Load Data", ButtonSizes.Large)]
    private void ButtonLoadData()
    {
        LoadData();
    }

    private void LoadData()
    {
        if(dataTable == null)
            dataTable = GetComponent<DataTableInspector>();
        dataTable.LoadData();
        // 각 데이터 클래스의 데이터를 로드
        Constant.LoadData();
        Hero.LoadData();
        //Enemy.LoadData();
        Stage.LoadData();



        IsLoaded = true;
        Debug.Log("DataBase Initialized and Data Loaded Successfully.");
    }

    
}
