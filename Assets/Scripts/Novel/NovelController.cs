using System;
using System.Linq;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    public event Action<AbstractNovelItemModel> OnModelActivated = delegate { };

    [SerializeField] protected NovelModel model = default;

    protected AbstractNovelItemModel lastSpeachModel = default;

    protected virtual void Awake()
    {
        model.Init();
    }

    protected virtual void Start()
    {
        lastSpeachModel = model.NovelItemsModel.FirstOrDefault();
        StepNovelProgression();
    }

    public virtual void StepNovelProgression()
    {
        foreach (AbstractNovelItemModel tempItemModel in model.NovelItemsModel)
        {
            if (tempItemModel.Equals(lastSpeachModel))
            {
                OnModelActivated(tempItemModel);
                lastSpeachModel = tempItemModel.TryGetProgressId(tempItemModel);
                //Debug.Log($"<color=green>Speach is found {tempItemModel.NameText} => {tempItemModel.SpeachText} </color>");
                break;
            }
            //else
            //    Debug.Log($"<color=red>Speach is found {tempItemModel.NameText} => {tempItemModel.SpeachText} </color>");

        }
    }
}
