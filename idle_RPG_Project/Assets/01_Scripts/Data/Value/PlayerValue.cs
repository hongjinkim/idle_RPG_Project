using UnityEngine;
using System.Numerics;

public class PlayerValue : BaseSaveValue<PlayerValue>
{
    public string NickName; // 닉네임
    public BigInteger Gold; // 골드
    public int Gem;    // 보석

    public void UpdateGold(BigInteger amount)
    {
        Gold += amount;
        if (Gold < 0) Gold = 0; // 골드가 음수가 되지 않도록
        MainSystem.Event.TriggerEvent(EEventType.GoldUpdated); // 골드 업데이트 이벤트 발생
        Save();
    }
    public void UpdateCrystal(int amount)
    {
        Gem += amount;
        if (Gem < 0) Gem = 0; // 보석이 음수가 되지 않도록
        MainSystem.Event.TriggerEvent(EEventType.GemUpdated); // 크리스탈 업데이트 이벤트 발생
        Save();
    }
}
