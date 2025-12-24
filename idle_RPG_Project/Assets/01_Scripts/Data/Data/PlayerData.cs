using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    //저장할 데이터 (서버에 주고 받을 데이터만 여기에 작성)
    [TabGroup("Tabs", "PlayerValue"), HideLabel][InlineProperty] public PlayerValue Value = new PlayerValue();
    [TabGroup("Tabs", "Hero"), HideLabel][InlineProperty] public HeroValue Hero = new HeroValue();

    // 데이터 로드, 추후 서버에서 받아오는 부분 구현 예정
    public void LoadData()
    {
        
    }

    
}
