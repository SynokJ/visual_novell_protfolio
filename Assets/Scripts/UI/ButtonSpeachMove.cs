using UnityEngine;

public class ButtonSpeachMove : AbstractActionButton
{
    [SerializeField] protected NovelController controller = default;

    protected override void OnClicked()
        => controller.StepNovelProgression();
}
