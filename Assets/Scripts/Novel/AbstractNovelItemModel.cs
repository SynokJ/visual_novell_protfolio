using System.Linq;
using UnityEngine;

public abstract class AbstractNovelItemModel : ScriptableObject
{
    public string NameText => nameText.Trim();
    public string SpeachText => speachText.Trim();
    public Sprite CharacterSprite => characterSprite;
    public Sprite BackgroundSprite => backgroundSprite;
    public string CharacterAnimTrigger => characterAnimTrigger.Trim();

    public Color EditorNodeColor => editorNodeColor;

    [SerializeField] protected string nameText = default;
    [SerializeField] protected string speachText = default;
    [SerializeField] protected Sprite characterSprite = default;
    [SerializeField] protected Sprite backgroundSprite = default;

    [Space, Header("fromLeft")]
    [SerializeField] protected string characterAnimTrigger = "fromLeft";

    [Space, Header("Editor Settings")]
    [SerializeField] protected Color editorNodeColor = new Color(0.22f, 0.22f, 0.22f, 1f);

    public abstract AbstractNovelItemModel TryGetProgressModel(AbstractNovelItemModel model);

    public override bool Equals(object other)
    {
        if (other is AbstractNovelItemModel novelModel)
            return this.name.Trim().Equals(novelModel.name.Trim());
        else
            return false;
    }

    public override int GetHashCode()
        => (NameText.Concat(SpeachText)).GetHashCode();
}
