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
        "Voice",
        "Question"
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

            if (tempSub.Equals(SUBJECTS[^1]))
                Debug.Log($"<color=orange>{tempSub} => {typedItems.Count()} => {typedItems[0].NameText}|{typedItems[0].SpeachText}</color>");
        }

        novelItemsModel = loadedItems.ToArray();
    }
}