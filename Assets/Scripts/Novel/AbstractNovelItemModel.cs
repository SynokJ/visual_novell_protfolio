using UnityEngine;

public abstract class AbstractNovelItemModel : ScriptableObject
{
    public string NameText => nameText;
    public string SpeachText => speachText;
    public Sprite CharacterSprite => characterSprite;
    public Sprite BackgroundSprite => backgroundSprite;
    public Identifier SpeachIdentifier => speachIdentifier;


    [SerializeField] protected string nameText = default;
    [SerializeField] protected string speachText = default;
    [SerializeField] protected Sprite characterSprite = default;
    [SerializeField] protected Sprite backgroundSprite = default;
    [SerializeField] protected Identifier speachIdentifier = default;

    public abstract Identifier TryGetProgressId(AbstractNovelItemModel model);
}
