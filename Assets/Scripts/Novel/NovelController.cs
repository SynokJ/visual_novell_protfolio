using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour
{
    public event Action<AbstractNovelItemModel> OnModelActivated = delegate { };

    [SerializeField] protected NovelModel model = default;
    [SerializeField] protected Button nextButton = default;

    [Space, Header("Choice Buttons:")]
    [SerializeField] protected Button firstChoiceButton = default;
    [SerializeField] protected Button secondChoiceButton = default;
    [SerializeField] protected Button thirdChoiceButton = default;

    protected AbstractNovelItemModel nextSpeachModel = default;
    protected NovelItemChoiceableModel currentSpeachChoiceableModel = default;

    protected virtual void Awake()
    {
        model.Init();
    }

    protected virtual void Start()
    {
        nextSpeachModel = model.NovelItemsModel.FirstOrDefault();
        StepNovelProgression();
    }

    public virtual void StepNovelProgression()
    {
        foreach (AbstractNovelItemModel tempItemModel in model.NovelItemsModel)
        {
            if (tempItemModel.Equals(nextSpeachModel))
            {
                OnModelActivated(tempItemModel);

                if (tempItemModel is NovelItemChoiceableModel choiceableModel)
                {
                    firstChoiceButton.onClick.AddListener(choiceableModel.FirstChoiceSelect);
                    firstChoiceButton.onClick.AddListener(UpdateByChoice);

                    secondChoiceButton.onClick.AddListener(choiceableModel.SecondChoiceSelect);
                    secondChoiceButton.onClick.AddListener(UpdateByChoice);

                    thirdChoiceButton.onClick.AddListener(choiceableModel.ThirdChoiceSelect);
                    thirdChoiceButton.onClick.AddListener(UpdateByChoice);

                    currentSpeachChoiceableModel = choiceableModel;
                    nextButton.gameObject.SetActive(false);
                }
                else
                {
                    firstChoiceButton.onClick.RemoveAllListeners();
                    secondChoiceButton.onClick.RemoveAllListeners();
                    thirdChoiceButton.onClick.RemoveAllListeners();
                    nextButton.gameObject.SetActive(true);
                    Switch(tempItemModel);
                }

                break;
            }
        }
    }

    protected virtual void UpdateByChoice()
    {
        nextSpeachModel = currentSpeachChoiceableModel.TryGetProgressModel(default);
        OnModelActivated(nextSpeachModel);

        firstChoiceButton.onClick.RemoveAllListeners();
        secondChoiceButton.onClick.RemoveAllListeners();
        thirdChoiceButton.onClick.RemoveAllListeners();
        nextButton.gameObject.SetActive(true);

        Switch(currentSpeachChoiceableModel);
    }

    protected virtual void Switch(AbstractNovelItemModel tempItemModel)
    {
        nextSpeachModel = tempItemModel.TryGetProgressModel(tempItemModel);
    }
}
