using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int Gold { get; set; }
    public int Gems { get; set; }

    public PlayerData()
    {
        Name = "NewPlayer";
        Level = 1;
        Experience = 0;
        Gold = 100;
        Gems = 10;
    }
}
