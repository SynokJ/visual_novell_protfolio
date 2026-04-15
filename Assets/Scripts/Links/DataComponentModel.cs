using UnityEngine;

public abstract class DataComponentModel : ScriptableObject
{
    public abstract GameObject DataObject { get; }
    public abstract Component DataComponentAsComponent { get; }

    public abstract void Clear();
}

public abstract class DataComponentModel<T> : DataComponentModel where T : Component
{
    public T DataComponent => dataComponent;

    public override GameObject DataObject => dataComponent != null
        ? dataComponent.gameObject
        : null;

    public override Component DataComponentAsComponent => dataComponent;

    [SerializeField] protected T dataComponent = default;

    public virtual void SetData(T value)
    {
        dataComponent = value;
    }

    public override void Clear()
    {
        dataComponent = null;
    }
}