using UnityEngine;

public abstract class AbstractDataComponentUploader<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected DataComponentModel<T> model = default;
    [SerializeField] protected T source = default;

    protected virtual void Awake()
    {
        Upload();
    }

    public virtual void SetSource(T value)
    {
        source = value;
    }

    public virtual void Upload()
    {
        if (model == null)
        {
            Debug.LogWarning($"{nameof(AbstractDataComponentUploader<T>)} on {name} has no model assigned.", this);
            return;
        }

        if (source == null)
        {
            Debug.LogWarning($"{nameof(AbstractDataComponentUploader<T>)} on {name} has no source assigned.", this);
            return;
        }

        model.SetData(source);
        OnUploaded(source);
    }

    protected virtual void OnUploaded(T uploadedData)
    {
    }
}
