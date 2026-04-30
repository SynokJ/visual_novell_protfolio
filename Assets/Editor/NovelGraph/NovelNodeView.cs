using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NovelNodeView : Node
{
    public AbstractNovelItemModel Model { get; private set; }
    public Port InputPort { get; private set; }

    public NovelNodeView(AbstractNovelItemModel model)
    {
        Model = model;

        title = model.name;

        ApplyNodeColor(model.EditorNodeColor);

        CreateInputPort();
        CreateMainInfo();

        if (model is NovelItemSimpleModel)
        {
            CreateOutputPort("Next", "nextModel");
        }
        else if (model is NovelItemChoiceableModel choiceableModel)
        {
            CreateOutputPort($"1: {choiceableModel.FirstChoiceText}", "firstChoiceModel");
            CreateOutputPort($"2: {choiceableModel.SecondChoiceText}", "secondChoiceModel");
            CreateOutputPort($"3: {choiceableModel.ThirdChoiceText}", "thirdChoiceModel");
        }

        RefreshExpandedState();
        RefreshPorts();
    }

    private void ApplyNodeColor(Color color)
    {
        titleContainer.style.backgroundColor = color;

        style.borderTopColor = color;
        style.borderBottomColor = color;
        style.borderLeftColor = color;
        style.borderRightColor = color;

        style.borderTopWidth = 2;
        style.borderBottomWidth = 2;
        style.borderLeftWidth = 2;
        style.borderRightWidth = 2;

        Color textColor = GetReadableTextColor(color);

        Label titleLabel = titleContainer.Q<Label>();
        if (titleLabel != null)
        {
            titleLabel.style.color = textColor;
            titleLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        }
    }

    private Color GetReadableTextColor(Color backgroundColor)
    {
        float brightness =
            backgroundColor.r * 0.299f +
            backgroundColor.g * 0.587f +
            backgroundColor.b * 0.114f;

        return brightness > 0.55f ? Color.black : Color.white;
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
        Label speakerLabel = new Label($"Name: {Model.NameText}");
        speakerLabel.style.whiteSpace = WhiteSpace.Normal;
        mainContainer.Add(speakerLabel);

        Label speechLabel = new Label($"Text: {Model.SpeachText}");
        speechLabel.style.whiteSpace = WhiteSpace.Normal;
        speechLabel.style.marginTop = 6;
        mainContainer.Add(speechLabel);

        Label typeLabel = new Label($"Type: {Model.GetType().Name}");
        typeLabel.style.marginTop = 6;
        typeLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        mainContainer.Add(typeLabel);
    }

    public Port GetOutputPort(string fieldName)
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