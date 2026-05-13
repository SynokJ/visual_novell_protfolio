using System.Linq;
using UnityEngine;

public abstract class AbstractNovelItemModel : AbstractNovelGraphNodeModel
{
    public string NameText => nameText.Trim();
    public string SpeachText => speachText.Trim();
    public Sprite CharacterSprite => characterSprite;
    public Sprite BackgroundSprite => backgroundSprite;
    public string CharacterAnimTrigger => characterAnimTrigger.Trim();
    public NovelChapterTransitionModel ChapterTransitionModel => chapterTransitionModel;

    [SerializeField] protected string nameText = default;
    [SerializeField] protected string speachText = default;
    [SerializeField] protected Sprite characterSprite = default;
    [SerializeField] protected Sprite backgroundSprite = default;

    [Space, Header("fromLeft")]
    [SerializeField] protected string characterAnimTrigger = "fromLeft";

    [SerializeField] protected NovelChapterTransitionModel chapterTransitionModel;

    public override bool Equals(object other)
    {
        if (other is AbstractNovelGraphNodeModel novelModel)
            return this.name.Trim().Equals(novelModel.name.Trim());
        else
            return false;
    }

    public override int GetHashCode()
        => (NameText.Concat(SpeachText)).GetHashCode();
}
