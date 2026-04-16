using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudioClick : AbstractDataComponentLoader<AudioManagerController>
{
    [SerializeField] protected Identifier audioId = default;

    protected Button button = default;
    protected AudioManagerController audioManagerController = default;

    private void OnEnable()
    {
        button.onClick.AddListener(OnCLick);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnCLick);
    }

    protected override void Awake()
    {
        base.Awake();
        button = GetComponent<Button>();
    }

    protected virtual void OnCLick()
    {
        if (audioManagerController.IsUnityNull() || audioId.IsUnityNull()) return;
        audioManagerController.PlayAudioById(audioId);
    }

    protected override void OnLoad(AudioManagerController data)
    {
        audioManagerController = data;
    }
}
