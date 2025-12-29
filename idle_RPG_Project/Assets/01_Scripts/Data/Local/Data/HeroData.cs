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
    public Dictionary<string, HeroValue> HeroDict = new Dictionary<string, HeroValue>();

    private int heroCount;
    public void LoadData()
    {
        var heroList = DataTable.Hero.GetList();
        heroCount = heroList.Count;

        for (int i = 0; i < heroCount; i++)
        {
            var hero = new HeroValue();

            hero.ID = heroList[i].ID;
            hero.Name = heroList[i].Name;
            hero.MaxHp = heroList[i].Hp;
            hero.Atk = heroList[i].Atk;
            hero.Def = heroList[i].Def;
            hero.CritRate = heroList[i].CritRate;
            hero.CritDmg = heroList[i].CritDmg;
            hero.AtkRange = heroList[i].AtkRange;
            hero.MoveSpd = heroList[i].MoveSpd;
            hero.AtkSpd = heroList[i].AtkSpd;

            HeroDict[hero.ID] = hero;
        }


    }

    public HeroValue GetValue(string id)
    {
        return HeroDict.TryGetValue(id, out var hero) ? hero : null;
    }
    public HeroValue GetClone(string id)
    {
        if(!HeroDict.TryGetValue(id, out var hero))
        {
            return null;
        }
        var clone = new HeroValue();

        clone.ID = id;
        clone.Name = hero.Name;
        clone.MaxHp = hero.MaxHp;
        clone.Atk = hero.Atk;
        clone.Def = hero.Def;
        clone.CritRate = hero.CritRate;
        clone.CritDmg = hero.CritDmg;
        clone.AtkSpd = hero.AtkSpd;
        clone.MoveSpd = hero.MoveSpd;
        clone.AtkRange = hero.AtkRange;

        return clone;
    }
}
