using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NovelController : MonoBehaviour
{
    public event Action<AbstractNovelGraphNodeModel> OnModelActivated = delegate { };
    public event Action<NovelChapterTransitionModel> OnTransitionActivated = delegate { };

    [SerializeField] protected NovelModel model = default;
    [SerializeField] protected NovelChapterTransitionModel startTransition = default;

    [SerializeField] protected Button nextButton = default;

    [Space, Header("Choice Buttons:")]
    [SerializeField] protected Button firstChoiceButton = default;
    [SerializeField] protected Button secondChoiceButton = default;
    [SerializeField] protected Button thirdChoiceButton = default;

    protected AbstractNovelGraphNodeModel nextSpeachModel = default;
    protected NovelItemChoiceableModel currentSpeachChoiceableModel = default;

    private NovelChapterTransitionModel currentTransition = default;
    private bool isShowingTransition;

    protected virtual void Awake()
    {
        model.Init();
    }

    protected virtual void Start()
    {
        if (startTransition != null)
        {
            ActivateTransition(startTransition);
        }
        else
        {
            nextSpeachModel = model.NovelItemsModel.FirstOrDefault();
            StepNovelProgression();
        }
    }

    public virtual void StepNovelProgression()
    {
        if (isShowingTransition)
        {
            FinishTransition();
            return;
        }

        if (nextSpeachModel is NovelChapterTransitionModel transitionModel)
        {
            ActivateTransition(transitionModel);
            return;
        }

        foreach (AbstractNovelItemModel tempItemModel in model.NovelItemsModel)
        {
            if (tempItemModel.Equals(nextSpeachModel))
            {
                OnModelActivated(tempItemModel);

                if (tempItemModel is NovelItemChoiceableModel choiceableModel)
                {
                    firstChoiceButton.onClick.RemoveAllListeners();
                    secondChoiceButton.onClick.RemoveAllListeners();
                    thirdChoiceButton.onClick.RemoveAllListeners();

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

    protected virtual void ActivateTransition(NovelChapterTransitionModel transitionModel)
    {
        currentTransition = transitionModel;
        isShowingTransition = true;

        nextButton.gameObject.SetActive(true);

        firstChoiceButton.onClick.RemoveAllListeners();
        secondChoiceButton.onClick.RemoveAllListeners();
        thirdChoiceButton.onClick.RemoveAllListeners();

        OnTransitionActivated(transitionModel);
    }

    protected virtual void FinishTransition()
    {
        if (currentTransition == null)
        {
            isShowingTransition = false;
            nextSpeachModel = model.NovelItemsModel.FirstOrDefault();
            StepNovelProgression();
            return;
        }

        nextSpeachModel = currentTransition.StartNovelModel;

        currentTransition = null;
        isShowingTransition = false;

        StepNovelProgression();
    }

    protected virtual void UpdateByChoice()
    {
        nextSpeachModel = currentSpeachChoiceableModel.TryGetProgressModel(default);
        OnModelActivated(nextSpeachModel);

        firstChoiceButton.onClick.RemoveAllListeners();
        secondChoiceButton.onClick.RemoveAllListeners();
        thirdChoiceButton.onClick.RemoveAllListeners();

        nextButton.gameObject.SetActive(true);

        Switch(nextSpeachModel);
    }

    protected virtual void Switch(AbstractNovelGraphNodeModel tempItemModel)
    {
        if (tempItemModel is AbstractNovelItemModel novelItemModel &&
            novelItemModel.ChapterTransitionModel != null)
        {
            nextSpeachModel = novelItemModel.ChapterTransitionModel;
            return;
        }

        nextSpeachModel = tempItemModel.TryGetProgressModel(tempItemModel);
    }
}