using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class AbstractActionButton : MonoBehaviour
{
    public event Action OnButtonClicked = delegate { };

    protected Button button = default;

    protected virtual void OnEnable()
        => button.onClick.AddListener(OnClicked);

    protected virtual void OnDisable()
        => button.onClick.RemoveListener(OnClicked);

    private void Awake()
        => button = GetComponent<Button>();

    protected abstract void OnClicked();
}
