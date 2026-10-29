using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelInterestBranchModel), menuName = "SOs/Novel/" + nameof(NovelInterestBranchModel))]
public class NovelInterestBranchModel : AbstractNovelGraphNodeModel
{
    [Header("Interest Branch:")]
    [SerializeField] protected int threshold = default;
    [SerializeField] protected AbstractNovelGraphNodeModel atOrAboveModel = default;
    [SerializeField] protected AbstractNovelGraphNodeModel belowModel = default;

    public AbstractNovelGraphNodeModel Resolve(int interest)
        => interest >= threshold ? atOrAboveModel : belowModel;

    // The branch depends on runtime state owned by the controller, which resolves it via Resolve().
    public override AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model)
        => belowModel;
}
