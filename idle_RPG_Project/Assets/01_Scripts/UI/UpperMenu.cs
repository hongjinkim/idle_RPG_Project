using TMPro;
using UnityEngine;
using System.Numerics;

public class UpperMenu : UIBase
{
    [SerializeField] private TextMeshProUGUI goldText;

    private BigInteger _currentDisplayGold = 0;

    public override void BindEvents()
    {
        MainSystem.Instance.Event.StartListening(EEventType.GoldChanged, RefreshGoldText);
    }
    protected override void UnbindEvents()
    {
        MainSystem.Instance.Event.StopListening(EEventType.GoldChanged, RefreshGoldText);
    }
    public override void RefreshUI()
    {
        RefreshGoldText(MainSystem.Instance.Player.playerData.Gold);
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
}
    