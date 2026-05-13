using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NovelController))]
public class NovelView : MonoBehaviour
{
    [Header("Text Components:")]
    [SerializeField] protected Text nameText = default;
    [SerializeField] protected Text speachText = default;
    [SerializeField] protected Text firstChoiceText = default;
    [SerializeField] protected Text secondChoiceText = default;
    [SerializeField] protected Text thirdChoiceText = default;

    [Space, Header("Image Components:")]
    [SerializeField] protected Image characterImage = default;
    [SerializeField] protected Image backgroundImage = default;

    [Space, Header("Choice Buttons:")]
    [SerializeField] protected Button firstChoiceButton = default;
    [SerializeField] protected Button secondChoiceButton = default;
    [SerializeField] protected Button thirdChoiceButton = default;

    [Space, Header("Choice Buttons:")]
    [SerializeField] protected Image transitionBackground = default;
    [SerializeField] protected Text transitionText = default;

    protected NovelController controller = default;
    protected Coroutine nameCoroutine = default;
    protected Coroutine speachCoroutine = default;
    protected bool isTransitioning = false;

    private Coroutine transitionFadeCoroutine;
    private Coroutine transitionTextCoroutine;

    protected virtual void OnEnable()
    {
        controller.OnModelActivated += OnModelActivate;
        controller.OnTransitionActivated += OnTransitionActivated;
    }


    protected virtual void OnDisable()
    {
        controller.OnModelActivated -= OnModelActivate;
        controller.OnTransitionActivated -= OnTransitionActivated;
    }

    protected virtual void Awake()
        => controller = GetComponent<NovelController>();

    private void OnTransitionActivated(NovelChapterTransitionModel model)
    {
        if (transitionFadeCoroutine != null)
        {
            StopCoroutine(transitionFadeCoroutine);
            transitionFadeCoroutine = null;
        }

        if (transitionTextCoroutine != null)
        {
            StopCoroutine(transitionTextCoroutine);
            transitionTextCoroutine = null;
        }

        transitionBackground.gameObject.SetActive(true);
        transitionBackground.enabled = true;

        transitionText.gameObject.SetActive(true);
        transitionText.enabled = true;

        SetTransitionAlpha(1f);

        transitionText.text = "";
        transitionTextCoroutine = StartCoroutine(SetTextWithDelay(transitionText, model.ChapterTitle));

        isTransitioning = true;
    }

    private void OnModelActivate(AbstractNovelGraphNodeModel otherModel)
    {
        if (!isTransitioning)
        {
            transitionBackground.enabled = false;
            transitionText.enabled = false;
        }
        else
        {
            if (transitionFadeCoroutine != null)
                StopCoroutine(transitionFadeCoroutine);

            transitionFadeCoroutine = StartCoroutine(SetVisibilityStatusWithDelay());
            isTransitioning = false;
        }

        if (otherModel is not AbstractNovelItemModel model) return;

        if (!nameCoroutine.IsUnityNull())
            StopCoroutine(nameCoroutine);
        nameCoroutine = StartCoroutine(SetTextWithDelay(nameText, model.NameText));

        if (!speachCoroutine.IsUnityNull())
            StopCoroutine(speachCoroutine);
        speachCoroutine = StartCoroutine(SetTextWithDelay(speachText, model.SpeachText));

        SetChoiceVisibility(false);
        if (model is NovelItemChoiceableModel choiceableModel)
        {
            firstChoiceText.text = choiceableModel.FirstChoiceText;
            secondChoiceText.text = choiceableModel.SecondChoiceText;
            thirdChoiceText.text = choiceableModel.ThirdChoiceText;
            SetChoiceVisibility(true);
        }

        characterImage.enabled = !model.CharacterSprite.IsUnityNull();
        characterImage.sprite = model.CharacterSprite;
        characterImage.preserveAspect = true;

        if (characterImage.TryGetComponent(out Animator animator))
            animator.SetTrigger(model?.CharacterAnimTrigger);

        backgroundImage.sprite = model.BackgroundSprite;
        backgroundImage.preserveAspect = true;
    }

    protected IEnumerator SetTextWithDelay(Text currentText, string textData)
    {
        string targetText = textData;
        string tempText = default;

        for (int i = 0; i < targetText.Length; i++)
        {
            tempText += targetText[i];
            yield return new WaitForEndOfFrame();
            currentText.text = tempText;
        }
    }

    protected IEnumerator SetVisibilityStatusWithDelay(bool status = false)
    {
        SetTransitionAlpha(1f);

        float duration = 0.6f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetTransitionAlpha(alpha);
            yield return null;
        }

        SetTransitionAlpha(0f);

        transitionBackground.enabled = status;
        transitionText.enabled = status;

        transitionFadeCoroutine = null;
    }

    private void SetTransitionAlpha(float alpha)
    {
        Color bgColor = transitionBackground.color;
        bgColor.a = alpha;
        transitionBackground.color = bgColor;

        Color textColor = transitionText.color;
        textColor.a = alpha;
        transitionText.color = textColor;
    }

    protected virtual void SetChoiceVisibility(bool status)
    {
        firstChoiceButton.gameObject.SetActive(status);
        secondChoiceButton.gameObject.SetActive(status);
        thirdChoiceButton.gameObject.SetActive(status);
    }
}
