using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

public class DataManager:BaseManager
{
    public bool IsLoaded { get; private set; }

    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty] public HeroData Hero = new HeroData();

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        LoadData();
        IsLoaded = true;

        Debug.Log("DataBase Initialized and Data Loaded Successfully.");
    }


    [Button("Load Data", ButtonSizes.Large)]
    private void LoadData()
    {
        // 각 데이터 클래스의 데이터를 로드
        Hero.LoadData();
        
    }

    
}
