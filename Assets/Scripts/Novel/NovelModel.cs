using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(NovelModel), menuName = "SOs/Novel/" + nameof(NovelModel))]
public class NovelModel : ScriptableObject
{
    public IReadOnlyCollection<AbstractNovelItemModel> NovelItemsParameter => novelItemsParameter;

    [SerializeField] protected AbstractNovelItemModel[] novelItemsParameter = default;
}

[System.Serializable]
public class NovelItemParameter
{
    public Identifier Id => id;
    public AbstractNovelItemModel ItemModel => itemModel;

    [SerializeField] protected Identifier id = default;
    [SerializeField] protected AbstractNovelItemModel itemModel = default;
}
