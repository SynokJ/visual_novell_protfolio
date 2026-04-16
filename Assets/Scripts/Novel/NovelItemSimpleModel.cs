using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelItemSimpleModel), menuName = "SOs/Novel/" + nameof(NovelItemSimpleModel))]
public class NovelItemSimpleModel : AbstractNovelItemModel
{
    [Space, Header("Next Speach Model:")]
    [SerializeField] protected AbstractNovelItemModel nextModel = default;

    public override AbstractNovelItemModel TryGetProgressId(AbstractNovelItemModel model)
        => nextModel;
}
