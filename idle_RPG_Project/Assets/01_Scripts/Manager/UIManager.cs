using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using NUnit.Framework;

public class UIManager : BaseManager
{
    public List<UIBase> UIList;

    protected override async UniTask OnInitialize()
    {
        Initialize();
        await UniTask.CompletedTask;
    }
    private void Initialize()
    {
        UIList = new List<UIBase>();
        UIList.AddRange(FindObjectsByType<UIBase>(FindObjectsSortMode.None));
        RefreshAllUI();
    }

    public void RefreshAllUI()
    {
        foreach (var ui in UIList)
        {
            ui.RefreshUI();
            ui.BindEvents();
        }
    }


}
