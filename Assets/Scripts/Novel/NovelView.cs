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

    protected NovelController controller = default;
    protected Coroutine nameCoroutine = default;
    protected Coroutine speachCoroutine = default;

    protected virtual void OnEnable()
        => controller.OnModelActivated += OnModelActivate;

    protected virtual void OnDisable()
        => controller.OnModelActivated -= OnModelActivate;

    protected virtual void Awake()
        => controller = GetComponent<NovelController>();

    private void OnModelActivate(AbstractNovelItemModel model)
    {
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

    protected virtual void SetChoiceVisibility(bool status)
    {
        firstChoiceButton.gameObject.SetActive(status);
        secondChoiceButton.gameObject.SetActive(status);
        thirdChoiceButton.gameObject.SetActive(status);
    }
}
