using UnityEngine;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = nameof(NovelItemChoiceableModel), menuName = "SOs/Novel/" + nameof(NovelItemChoiceableModel))]
public class NovelItemChoiceableModel : AbstractNovelItemModel
{
    public string FirstChoiceText => firstChoiceText;
    public string SecondChoiceText => secondChoiceText;
    public string ThirdChoiceText => thirdChoiceText;

    [Header("Choice Data Ids:")]
    [SerializeField] protected AbstractNovelGraphNodeModel firstChoiceModel = default;
    [SerializeField] protected AbstractNovelGraphNodeModel secondChoiceModel = default;
    [SerializeField] protected AbstractNovelGraphNodeModel thirdChoiceModel = default;

    [Space, Header("Choice Text Data")]
    [SerializeField] protected string firstChoiceText = default;
    [SerializeField] protected string secondChoiceText = default;
    [SerializeField] protected string thirdChoiceText = default;

    protected AbstractNovelGraphNodeModel selectedChoiceModel = default;

    public virtual void FirstChoiceSelect()
    {
        selectedChoiceModel = firstChoiceModel;
        TryGetProgressModel(default);
        Debug.Log("FirstChoiceSelect");
    }

    public virtual void SecondChoiceSelect()
    {
        selectedChoiceModel = secondChoiceModel;
        TryGetProgressModel(default);
        Debug.Log("SecondChoiceSelect");
    }

    public virtual void ThirdChoiceSelect()
    {
        selectedChoiceModel = thirdChoiceModel;
        TryGetProgressModel(default);
        Debug.Log("ThirdChoiceSelect");
    }

    public override AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model)
        => selectedChoiceModel.IsUnityNull() ? firstChoiceModel : selectedChoiceModel;
}
