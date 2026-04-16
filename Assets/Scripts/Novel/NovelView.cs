using System;
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

    protected virtual void OnEnable()
        => controller.OnModelActivated += OnModelActivate;

    protected virtual void OnDisable()
        => controller.OnModelActivated -= OnModelActivate;

    protected virtual void Awake()
        => controller = GetComponent<NovelController>();

    private void OnModelActivate(AbstractNovelItemModel model)
    {
        nameText.text = model.NameText;
        speachText.text = model.SpeachText;

        SetChoiceVisibility(false);
        if (model is NovelItemChoiceableModel choiceableModel)
        {
            firstChoiceText.text = choiceableModel.FirstChoiceText;
            secondChoiceText.text = choiceableModel.SecondChoiceText;
            thirdChoiceText.text = choiceableModel.ThirdChoiceText;
            SetChoiceVisibility(true);
        }

        characterImage.sprite = model.CharacterSprite;
        backgroundImage.sprite = model.BackgroundSprite;
    }

    protected virtual void SetChoiceVisibility(bool status)
    {
        firstChoiceButton.gameObject.SetActive(status);
        secondChoiceButton.gameObject.SetActive(status);
        thirdChoiceButton.gameObject.SetActive(status);
    }
}
