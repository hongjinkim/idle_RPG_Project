using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UpgradeData
{
    [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout, KeyLabel = "Upgrade ID", ValueLabel = "Info")]
    public Dictionary<string, UpgradeValue> UpgradeDict = new Dictionary<string, UpgradeValue>();

    public void LoadData()
    {
        


    }

}
