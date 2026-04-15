using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelItemSimpleModel), menuName = "SOs/Novel/" + nameof(NovelItemSimpleModel))]
public class NovelItemSimpleModel : AbstractNovelItemModel
{
    public Identifier NextModelId => nextModelId;

    [Space, Header("Next Speach Model:")]
    [SerializeField] protected Identifier nextModelId = default;

    public override Identifier TryGetProgressId(AbstractNovelItemModel model)
        => nextModelId;
}
