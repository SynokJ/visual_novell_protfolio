using UnityEngine;

public abstract class AbstractDataComponentLoader<T> : MonoBehaviour where T : Component
{
    public T LoadedData => model != null ? model.DataComponent : null;

    [SerializeField] protected DataComponentModel<T> model = default;

    protected virtual void Awake()
    {
        Load();
    }

    public virtual void Load()
    {
        if (model == null)
        {
            Debug.LogWarning($"{nameof(AbstractDataComponentLoader<T>)} on {name} has no model assigned.", this);
            return;
        }

        var data = model.DataComponent;

        if (data == null)
        {
            Debug.LogWarning($"{model.name} does not contain {typeof(T).Name} data.", this);
            return;
        }

        OnLoad(data);
    }

    protected abstract void OnLoad(T data);
}
