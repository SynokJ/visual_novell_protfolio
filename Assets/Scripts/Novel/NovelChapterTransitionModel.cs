using UnityEngine;

[CreateAssetMenu(menuName = "SOs/Transition/" + nameof(NovelChapterTransitionModel), fileName = nameof(NovelChapterTransitionModel))]
public class NovelChapterTransitionModel : AbstractNovelGraphNodeModel
{
    public string ChapterTitle => chapterTitle;
    public AbstractNovelItemModel StartNovelModel => startNovelModel;

    [SerializeField] protected string chapterTitle = default;
    [SerializeField] protected AbstractNovelItemModel startNovelModel = default;

    public override AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model)
        => startNovelModel;
}
