using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NovelGraphView : GraphView
{
    private readonly Dictionary<string, NovelNodeView> nodeViews = new();

    private bool isLoadingGraph;

    public NovelGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        AddGridBackground();

        graphViewChanged += OnGraphViewChanged;
    }

    private void AddGridBackground()
    {
        GridBackground grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
    }

    public void LoadExistingNovelItems()
    {
        isLoadingGraph = true;
        graphViewChanged -= OnGraphViewChanged;

        ClearGraphVisuals();
        nodeViews.Clear();

        List<AbstractNovelItemModel> items = LoadAllNovelItems();

        for (int i = 0; i < items.Count; i++)
        {
            AbstractNovelItemModel item = items[i];

            NovelNodeView nodeView = new NovelNodeView(item);

            Rect savedPosition = GetSavedNodePosition(item, i);
            nodeView.SetPosition(savedPosition);

            AddElement(nodeView);

            string path = AssetDatabase.GetAssetPath(item);
            nodeViews[path] = nodeView;
        }

        GenerateLinks();

        graphViewChanged += OnGraphViewChanged;
        isLoadingGraph = false;

        Debug.Log("Graph refreshed safely.");
    }

    private void ClearGraphVisuals()
    {
        foreach (Edge edge in edges.ToList())
        {
            edge.output?.Disconnect(edge);
            edge.input?.Disconnect(edge);
            RemoveElement(edge);
        }

        foreach (Node node in nodes.ToList())
        {
            RemoveElement(node);
        }
    }

    private List<AbstractNovelItemModel> LoadAllNovelItems()
    {
        List<AbstractNovelItemModel> result = new();

        string[] simpleGuids = AssetDatabase.FindAssets("t:NovelItemSimpleModel");
        string[] choiceGuids = AssetDatabase.FindAssets("t:NovelItemChoiceableModel");

        foreach (string guid in simpleGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NovelItemSimpleModel item = AssetDatabase.LoadAssetAtPath<NovelItemSimpleModel>(path);

            if (item != null)
                result.Add(item);
        }

        foreach (string guid in choiceGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NovelItemChoiceableModel item = AssetDatabase.LoadAssetAtPath<NovelItemChoiceableModel>(path);

            if (item != null)
                result.Add(item);
        }

        return result
            .Where(x => x != null)
            .OrderBy(x => AssetDatabase.GetAssetPath(x))
            .ToList();
    }

    private void GenerateLinks()
    {
        foreach (NovelNodeView sourceNode in nodeViews.Values)
        {
            AbstractNovelItemModel sourceModel = sourceNode.Model;

            if (sourceModel is NovelItemSimpleModel)
            {
                CreateLinkFromSerializedField(sourceNode, "nextModel");
            }
            else if (sourceModel is NovelItemChoiceableModel)
            {
                CreateLinkFromSerializedField(sourceNode, "firstChoiceModel");
                CreateLinkFromSerializedField(sourceNode, "secondChoiceModel");
                CreateLinkFromSerializedField(sourceNode, "thirdChoiceModel");
            }
        }
    }

    private void CreateLinkFromSerializedField(NovelNodeView sourceNode, string fieldName)
    {
        AbstractNovelItemModel targetModel = GetModelReference(sourceNode.Model, fieldName);

        if (targetModel == null)
            return;

        string targetPath = AssetDatabase.GetAssetPath(targetModel);

        if (string.IsNullOrEmpty(targetPath))
        {
            Debug.LogWarning($"Target model has no asset path: {targetModel.name}");
            return;
        }

        if (!nodeViews.TryGetValue(targetPath, out NovelNodeView targetNode))
        {
            Debug.LogWarning($"Target node not found in graph: {targetModel.name} at {targetPath}");
            return;
        }

        Port outputPort = sourceNode.GetOutputPort(fieldName);
        Port inputPort = targetNode.InputPort;

        if (outputPort == null || inputPort == null)
        {
            Debug.LogWarning($"Port missing for link: {sourceNode.Model.name}.{fieldName}");
            return;
        }

        Edge edge = new Edge
        {
            output = outputPort,
            input = inputPort
        };

        edge.output.Connect(edge);
        edge.input.Connect(edge);

        AddElement(edge);

        Debug.Log($"DRAW LINK: {sourceNode.Model.name}.{fieldName} -> {targetModel.name}");
    }

    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (isLoadingGraph)
            return graphViewChange;

        if (graphViewChange.movedElements != null)
        {
            foreach (GraphElement element in graphViewChange.movedElements)
            {
                if (element is NovelNodeView movedNode)
                {
                    SaveNodePosition(movedNode);
                }
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (Edge edge in graphViewChange.edgesToCreate)
            {
                NovelNodeView sourceNode = edge.output?.node as NovelNodeView;
                NovelNodeView targetNode = edge.input?.node as NovelNodeView;

                if (sourceNode == null || targetNode == null)
                    continue;

                string fieldName = edge.output.userData as string;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                SetModelReference(sourceNode.Model, fieldName, targetNode.Model);
            }
        }

        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                if (element is not Edge edge)
                    continue;

                NovelNodeView sourceNode = edge.output?.node as NovelNodeView;
                NovelNodeView targetNode = edge.input?.node as NovelNodeView;

                if (sourceNode == null || targetNode == null)
                    continue;

                string fieldName = edge.output.userData as string;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                AbstractNovelItemModel currentTarget = GetModelReference(sourceNode.Model, fieldName);

                if (currentTarget == targetNode.Model)
                {
                    SetModelReference(sourceNode.Model, fieldName, null);
                }
            }
        }

        return graphViewChange;
    }

    private string GetNodePositionKey(AbstractNovelItemModel model)
    {
        string assetPath = AssetDatabase.GetAssetPath(model);
        return $"NovelGraph_NodePosition_{assetPath}";
    }

    private Rect GetSavedNodePosition(AbstractNovelItemModel model, int index)
    {
        string key = GetNodePositionKey(model);

        if (EditorPrefs.HasKey(key + "_x") && EditorPrefs.HasKey(key + "_y"))
        {
            float savedX = EditorPrefs.GetFloat(key + "_x");
            float savedY = EditorPrefs.GetFloat(key + "_y");
            return new Rect(savedX, savedY, 280, 180);
        }

        float x = 100 + (index % 4) * 330;
        float y = 100 + (index / 4) * 260;

        return new Rect(x, y, 280, 180);
    }

    private void SaveNodePosition(NovelNodeView nodeView)
    {
        if (nodeView == null || nodeView.Model == null)
            return;

        Rect position = nodeView.GetPosition();
        string key = GetNodePositionKey(nodeView.Model);

        EditorPrefs.SetFloat(key + "_x", position.x);
        EditorPrefs.SetFloat(key + "_y", position.y);

        Debug.Log($"SAVED POSITION: {nodeView.Model.name} -> {position.x}, {position.y}");
    }

    private AbstractNovelItemModel GetModelReference(AbstractNovelItemModel sourceModel, string fieldName)
    {
        SerializedObject serializedObject = new SerializedObject(sourceModel);
        serializedObject.Update();

        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property == null)
            return null;

        return property.objectReferenceValue as AbstractNovelItemModel;
    }

    private void SetModelReference(AbstractNovelItemModel sourceModel, string fieldName, AbstractNovelItemModel targetModel)
    {
        if (sourceModel == null)
        {
            Debug.LogError("Source model is null.");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(sourceModel);
        serializedObject.Update();

        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property == null)
        {
            Debug.LogError($"Field not found: {fieldName} on {sourceModel.name}. Type: {sourceModel.GetType().Name}");
            return;
        }

        Undo.RecordObject(sourceModel, "Update Novel Link");

        property.objectReferenceValue = targetModel;

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(sourceModel);
        AssetDatabase.SaveAssets();

        Debug.Log($"SAVED LINK: {sourceModel.name}.{fieldName} -> {(targetModel != null ? targetModel.name : "null")}");
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new();

        ports.ForEach(port =>
        {
            bool isDifferentNode = port.node != startPort.node;
            bool isDifferentDirection = port.direction != startPort.direction;

            if (isDifferentNode && isDifferentDirection)
            {
                compatiblePorts.Add(port);
            }
        });

        return compatiblePorts;
    }
}