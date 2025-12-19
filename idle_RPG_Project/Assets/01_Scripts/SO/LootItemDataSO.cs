using UnityEngine;

public enum ELootType
{
    Gold,
    Key,
    Gem,
    // 나중에 계속 추가 가능
}


[CreateAssetMenu(fileName = "LootItemSO", menuName = "Scriptable Objects/LootItemSO")]
public class LootItemDataSO : ScriptableObject
{
    public ELootType type;
    public Sprite iconSprite;       // 아이템 이미지
    public Color effectColor;       // 트레일 및 글로우 색상
    public float scale = 1.0f;      // 아이템마다 크기가 다를 수 있음
}
