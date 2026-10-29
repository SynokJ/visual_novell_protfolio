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

    [Space, Header("Interest Change (Timur):")]
    [SerializeField] protected int firstChoiceInterest = default;
    [SerializeField] protected int secondChoiceInterest = default;
    [SerializeField] protected int thirdChoiceInterest = default;

    // Interest change of the picked choice, 0 when nothing is picked.
    public int SelectedInterestChange => selectedInterestChange;

    // Runtime-only state: ScriptableObject fields persist between play sessions in the editor,
    // so the selection must be cleared once consumed (see ResetSelection).
    protected AbstractNovelGraphNodeModel selectedChoiceModel = default;
    protected int selectedInterestChange;
    protected bool hasSelection;

    public virtual void FirstChoiceSelect()
        => Select(firstChoiceModel, firstChoiceInterest);

    public virtual void SecondChoiceSelect()
        => Select(secondChoiceModel, secondChoiceInterest);

    public virtual void ThirdChoiceSelect()
        => Select(thirdChoiceModel, thirdChoiceInterest);

    public virtual void ResetSelection()
    {
        selectedChoiceModel = null;
        selectedInterestChange = 0;
        hasSelection = false;
    }

    public override AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model)
        => hasSelection ? selectedChoiceModel : firstChoiceModel;

    private void Select(AbstractNovelGraphNodeModel choiceModel, int interestChange)
    {
        selectedChoiceModel = choiceModel;
        selectedInterestChange = interestChange;
        hasSelection = true;
    }
}
