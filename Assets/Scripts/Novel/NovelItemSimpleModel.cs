using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelItemSimpleModel), menuName = "SOs/Novel/" + nameof(NovelItemSimpleModel))]
public class NovelItemSimpleModel : AbstractNovelItemModel
{
    [Space, Header("Next Speach Model:")]
    [SerializeField] protected AbstractNovelGraphNodeModel nextModel = default;

    public override AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model)
        => nextModel;
}
