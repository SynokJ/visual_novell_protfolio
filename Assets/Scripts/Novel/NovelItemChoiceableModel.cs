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

    public virtual void FirstChoiceSelect()
    {
        selectedChoiceModel = firstChoiceModel;
        TryGetProgressModel(default);
    }

    public virtual void SecondChoiceSelect()
    {
        selectedChoiceModel = secondChoiceModel;
        TryGetProgressModel(default);
    }

    public virtual void ThirdChoiceSelect()
    {
        selectedChoiceModel = thirdChoiceModel;
        TryGetProgressModel(default);
    }

    public override AbstractNovelItemModel TryGetProgressModel(AbstractNovelItemModel model)
        => selectedChoiceModel.IsUnityNull() ? firstChoiceModel : selectedChoiceModel;
}
