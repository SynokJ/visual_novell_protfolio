using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelModel), menuName = "SOs/Novel/" + nameof(NovelModel))]
public class NovelModel : ScriptableObject
{
    protected const string NOVEL_MODELS_PATH = "SOs/NovelItems/";

    protected readonly string[] SUBJECTS =
    {
        "Teacher",
        "Thoughts",
        "Timur",
        "Voice"
    };

    public IReadOnlyCollection<AbstractNovelItemModel> NovelItemsModel => novelItemsModel;

    protected AbstractNovelItemModel[] novelItemsModel;

    public virtual void Init()
    {
        List<AbstractNovelItemModel> loadedItems = new();

        foreach (string tempSub in SUBJECTS)
        {
            string path = NOVEL_MODELS_PATH + tempSub;
            AbstractNovelItemModel[] typedItems = Resources.LoadAll<AbstractNovelItemModel>(path);
            loadedItems.AddRange(typedItems);
        }

        novelItemsModel = loadedItems.ToArray();
    }
}