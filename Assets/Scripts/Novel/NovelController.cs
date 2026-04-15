using System;
using System.Linq;
using UnityEngine;

public class NovelController : MonoBehaviour
{
    public event Action<AbstractNovelItemModel> OnModelActivated = delegate { };

    [SerializeField] protected NovelModel model = default;

    protected Identifier lastSpeachId = default;

    private void Start()
    {
        lastSpeachId = model.NovelItemsParameter.FirstOrDefault().SpeachIdentifier;
        StepNovelProgression();
    }

    public virtual void StepNovelProgression()
    {
        foreach (AbstractNovelItemModel tempItemParameter in model.NovelItemsParameter)
        {
            if (tempItemParameter.SpeachIdentifier == lastSpeachId)
            {
                OnModelActivated(tempItemParameter);
                lastSpeachId = tempItemParameter.TryGetProgressId(tempItemParameter);
                break;
            }
        }
    }
}
