using UnityEngine;

public abstract class AbstractNovelGraphNodeModel : ScriptableObject
{
    public Color EditorNodeColor => editorNodeColor;

    [Space, Header("Editor Settings")]
    [SerializeField] protected Color editorNodeColor = new Color(0.22f, 0.22f, 0.22f, 1f);

    public abstract AbstractNovelGraphNodeModel TryGetProgressModel(AbstractNovelGraphNodeModel model);
}