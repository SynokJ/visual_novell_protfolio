using UnityEngine;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = nameof(NovelItemChoiceableModel), menuName = "SOs/Novel/" + nameof(NovelItemChoiceableModel))]
public class NovelItemChoiceableModel : AbstractNovelItemModel
{
    public string FirstChoiceText => firstChoiceText;
    public string SecondChoiceText => secondChoiceText;
    public string ThirdChoiceText => thirdChoiceText;

    [Header("Choice Data Ids:")]
    [SerializeField] protected AbstractNovelItemModel firstChoiceModel = default;
    [SerializeField] protected AbstractNovelItemModel secondChoiceModel = default;
    [SerializeField] protected AbstractNovelItemModel thirdChoiceModel = default;

    [Space, Header("Choice Text Data")]
    [SerializeField] protected string firstChoiceText = default;
    [SerializeField] protected string secondChoiceText = default;
    [SerializeField] protected string thirdChoiceText = default;

    protected AbstractNovelItemModel selectedChoiceModel = default;

    public override AbstractNovelItemModel TryGetProgressId(AbstractNovelItemModel model)
        => selectedChoiceModel.IsUnityNull() ? firstChoiceModel : selectedChoiceModel;
}
