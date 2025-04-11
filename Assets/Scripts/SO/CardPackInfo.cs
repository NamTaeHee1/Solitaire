using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CardPack", menuName = "CardPack")]
public class CardPackInfo : ScriptableObject
{
    [Header("Card Pack AssetReferenceSprite")]
    public AssetReferenceSprite cardPackSheet;

    [Header("Card Pack 프로필")]
    public Sprite cardPackProfile;

    [Header("Card Pack 이름")]
    public string cardPackName;

    [Header("Card Pack 유형")]
    public ECARD_PACK_TYPE cardPackType;
}