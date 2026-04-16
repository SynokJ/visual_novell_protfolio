using UnityEngine;
using Unity.VisualScripting;

public class NovelChoiceableModel : AbstractNovelItemModel
{
    [Header("Choice Data Ids:")]
    [SerializeField] protected Identifier firstChoiceID = default;
    [SerializeField] protected Identifier secondChoiceID = default;
    [SerializeField] protected Identifier thirdChoiceID = default;

    protected Identifier selectedChoiceID = default;

    public override Identifier TryGetProgressId(AbstractNovelItemModel model)
        => selectedChoiceID.IsUnityNull() ? firstChoiceID : selectedChoiceID;
}
