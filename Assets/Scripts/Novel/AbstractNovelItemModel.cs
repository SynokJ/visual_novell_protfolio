using System.Linq;
using UnityEngine;

public abstract class AbstractNovelItemModel : ScriptableObject
{
    public string NameText => nameText.Trim();
    public string SpeachText => speachText.Trim();
    public Sprite CharacterSprite => characterSprite;
    public Sprite BackgroundSprite => backgroundSprite;

    [SerializeField] protected string nameText = default;
    [SerializeField] protected string speachText = default;
    [SerializeField] protected Sprite characterSprite = default;
    [SerializeField] protected Sprite backgroundSprite = default;

    public abstract AbstractNovelItemModel TryGetProgressModel(AbstractNovelItemModel model);

    public override bool Equals(object other)
    {
        if (other is AbstractNovelItemModel novelModel)
            return NameText.Equals(novelModel.NameText) && SpeachText.Equals(novelModel.SpeachText);
        else
            return false;
    }

    public override int GetHashCode()
        => (NameText.Concat(SpeachText)).GetHashCode();
}
