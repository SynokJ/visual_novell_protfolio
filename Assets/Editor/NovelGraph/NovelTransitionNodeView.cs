using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NovelTransitionNodeView : NovelBaseNodeView
{
    public NovelChapterTransitionModel Model { get; private set; }

    public override AbstractNovelGraphNodeModel GraphModel => Model;

    public NovelTransitionNodeView(NovelChapterTransitionModel model)
    {
        Model = model;
        title = model.name;

        CreateInputPort();
        CreateMainInfo();
        CreateOutputPort("Start Novel", "startNovelModel");

        RefreshExpandedState();
        RefreshPorts();
    }

    private void CreateInputPort()
    {
        InputPort = InstantiatePort(
            Orientation.Horizontal,
            Direction.Input,
            Port.Capacity.Multi,
            typeof(bool)
        );

        InputPort.portName = "Input";
        inputContainer.Add(InputPort);
    }

    private void CreateOutputPort(string portLabel, string fieldName)
    {
        Port outputPort = InstantiatePort(
            Orientation.Horizontal,
            Direction.Output,
            Port.Capacity.Single,
            typeof(bool)
        );

        outputPort.portName = portLabel;
        outputPort.userData = fieldName;

        outputContainer.Add(outputPort);
    }

    private void CreateMainInfo()
    {
        Label chapterLabel = new Label($"Chapter: {Model.ChapterTitle}");
        chapterLabel.style.whiteSpace = WhiteSpace.Normal;
        mainContainer.Add(chapterLabel);

        Label typeLabel = new Label("Type: Chapter Transition");
        typeLabel.style.marginTop = 6;
        typeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        mainContainer.Add(typeLabel);
    }

    public override Port GetOutputPort(string fieldName)
    {
        foreach (VisualElement child in outputContainer.Children())
        {
            if (child is Port port && port.userData as string == fieldName)
            {
                return port;
            }
        }

        return null;
    }
}