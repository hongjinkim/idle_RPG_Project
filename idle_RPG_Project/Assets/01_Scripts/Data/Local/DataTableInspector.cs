using Sirenix.OdinInspector;
using System.Collections.Generic;
using UGS;
using UnityEngine;

public class DataTableInspector : MonoBehaviour
{
    [TabGroup("Tabs", "GlobalConstant")][ShowInInspector] public Dictionary<string, DataTable.GlobalConstant> Constant = new Dictionary<string, DataTable.GlobalConstant>();
    [TabGroup("Tabs", "Stage")][ShowInInspector] public Dictionary<string, DataTable.Stage> Stage = new Dictionary<string, DataTable.Stage>();
    [TabGroup("Tabs", "Hero")][ShowInInspector] public Dictionary<string, DataTable.Hero> Hero = new Dictionary<string, DataTable.Hero>();



    [Button("Load DataTable", ButtonSizes.Large)]
    public void LoadData()
    {
        //UnityGoogleSheet.LoadAllData();

        Constant = DataTable.GlobalConstant.GetDictionary();
        Stage = DataTable.Stage.GetDictionary();
        Hero = DataTable.Hero.GetDictionary();
    }
}


