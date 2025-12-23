using System.Numerics;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public BigInteger Experience { get; set; }
    public BigInteger Gold { get; set; }
    public int Gem { get; set; }

    public PlayerData()
    {
        Name = "NewPlayer";
        Level = 1;
        Experience = 0;
        Gold = 100;
        Gem = 10;
    }
}
