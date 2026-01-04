using TMPro;
using UnityEngine;
using System.Numerics;

public class UpperMenu : UIBase
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private TextMeshProUGUI combatPowerText;
    //[SerializeField] private TextMeshProUGUI rankingText;

    private BigInteger _currentDisplayGold = 0;
    private BigInteger _currentDisplayGem = 0;
    private BigInteger _currentDisplayPower = 0;

    public override void BindEvents()
    {
        MainSystem.Event.StartListening(EEventType.GoldUpdated, RefreshGoldText);
        MainSystem.Event.StartListening(EEventType.GemUpdated, RefreshGemText);
        MainSystem.Event.StartListening(EEventType.PlayerPowerUpdated, RefreshPowerText);
    }
    protected override void UnbindEvents()
    {
        MainSystem.Event.StopListening(EEventType.GoldUpdated, RefreshGoldText);
        MainSystem.Event.StopListening(EEventType.GemUpdated, RefreshGemText);
        MainSystem.Event.StopListening(EEventType.PlayerPowerUpdated, RefreshPowerText);
    }
    public override void RefreshUI()
    {
        RefreshGoldText(MainSystem.Player.playerData.Value.Gold);
        RefreshGemText(MainSystem.Player.playerData.Value.Gem);
        RefreshPowerText(MainSystem.Battle.MainHero.CombatPower);
    }

    private void RefreshGoldText(object data)
    {
        if (data is BigInteger newGold)
        {
            // 값이 바뀌었으므로 롤링 효과와 함께 갱신
            goldText.UpdateText(
                _currentDisplayGold,
                newGold,
                ETextTransitionType.Rolling,
                ENumberFormatType.Korean
            );

            _currentDisplayGold = newGold;
        }
        else
        {
            Debug.LogError("Invalid data type for RefreshGoldText");
        }
    }
    private void RefreshGemText(object data)
    {
        if(data is BigInteger newGem)
        {
            gemText.UpdateText(
                _currentDisplayGem,
                newGem,
                ETextTransitionType.Rolling,
                ENumberFormatType.Korean
             );

            _currentDisplayGem = newGem;
        }
        else
        {
            Debug.LogError("Invalid data type for RefreshGemText");
        }
    }

    private void RefreshPowerText(object data)
    {
        if (data is BigInteger newPower)
        {
            combatPowerText.UpdateText(
                _currentDisplayPower,
                newPower,
                ETextTransitionType.Rolling,
                ENumberFormatType.Korean
             );

            _currentDisplayPower = newPower;
        }
        else
        {
            Debug.LogError("Invalid data type for RefreshPowerText");
        }
    }
}
    