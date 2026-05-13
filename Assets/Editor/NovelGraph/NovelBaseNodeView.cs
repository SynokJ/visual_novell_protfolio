using UnityEditor.Experimental.GraphView;

public abstract class NovelBaseNodeView : Node
{
    public abstract AbstractNovelGraphNodeModel GraphModel { get; }
    public Port InputPort { get; protected set; }

    public abstract Port GetOutputPort(string fieldName);
}