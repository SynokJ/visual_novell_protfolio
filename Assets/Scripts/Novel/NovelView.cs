using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(NovelController))]
public class NovelView : MonoBehaviour
{
    [SerializeField] protected Text nameText = default;
    [SerializeField] protected Text speachText = default;
    [SerializeField] protected Text firstChoiceText = default;
    [SerializeField] protected Text secondChoiceText = default;
    [SerializeField] protected Text thirdChoiceText = default;
    [SerializeField] protected Image characterImage = default;
    [SerializeField] protected Image backgroundImage = default;

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

        firstChoiceText.text = "default";
        secondChoiceText.text = "default";
        thirdChoiceText.text = "default";

        characterImage.sprite = model.CharacterSprite;
        backgroundImage.sprite = model.BackgroundSprite;
    }
}
