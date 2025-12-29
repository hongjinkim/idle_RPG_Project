using UnityEngine;
using System.Numerics;

public class PlayerValue : BaseSaveValue<PlayerValue>
{
    public int CurrentStage = 1; // 현재 스테이지
    public string NickName; // 닉네임
    public BigInteger Gold; // 골드
    public BigInteger Gem;    // 보석
    public BigInteger CombatPower; // 전투력

    public void UpdateGold(BigInteger amount)
    {
        Gold += amount;
        if (Gold < 0) Gold = 0; // 골드가 음수가 되지 않도록
        MainSystem.Event.TriggerEvent(EEventType.GoldUpdated, Gold); // 골드 업데이트 이벤트 발생
        Save();
    }
    public void UpdateGem(BigInteger amount)
    {
        Gem += amount;
        if (Gem < 0) Gem = 0; // 보석이 음수가 되지 않도록
        MainSystem.Event.TriggerEvent(EEventType.GemUpdated, Gem); // 크리스탈 업데이트 이벤트 발생
        Save();
    }
    public void UpdateCombatPower(BigInteger amount)
    {
        CombatPower += amount;
        MainSystem.Event.TriggerEvent(EEventType.PlayerPowerUpdated, CombatPower);
        Save();
    }
}
