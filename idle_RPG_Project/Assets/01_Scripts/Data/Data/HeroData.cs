using Newtonsoft.Json.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroData
{
    [ShowInInspector, DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.Foldout, KeyLabel = "Hero ID", ValueLabel = "Info")]
    public Dictionary<int, HeroValue> HeroDict = new Dictionary<int, HeroValue>();

    private int heroCount;
    public void LoadData()
    {
        var heroList = DataTable.Hero.GetList();
        heroCount = heroList.Count;
        
        for (int i = 0; i < heroCount; i++)
        {
           var hero = new HeroValue();

            hero.Id = heroList[i].ID;
            hero.Name = heroList[i].Name;
            hero.MaxHp = heroList[i].MaxHp;
            hero.Atk = heroList[i].Atk;
            hero.Def = heroList[i].Def;
            hero.CritRate = heroList[i].CritRate;
            hero.CritDmg = heroList[i].CritDmg;
            hero.AtkRange = heroList[i].AtkRange;
            hero.MoveSpd = heroList[i].MoveSpd;
            hero.AtkSpd = heroList[i].AtkSpd;

            HeroDict[hero.Id] = hero;
        }

        
    }

    public HeroValue GetValue(int id)
    {
        return HeroDict.TryGetValue(id, out var hero) ? hero : null;
    }
}
