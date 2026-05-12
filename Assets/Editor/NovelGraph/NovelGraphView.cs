using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class NovelGraphView : GraphView
{
    private readonly Dictionary<string, NovelNodeView> nodeViews = new();
    private readonly Dictionary<string, NovelTransitionNodeView> nodeTransitionViews = new();

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
        nodeTransitionViews.Clear();

        List<AbstractNovelItemModel> items = LoadAllNovelItems();
        List<NovelChapterTransitionModel> chapterTransitions = LoadAllNovelTransitions();

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

        for (int i = 0; i < chapterTransitions.Count; i++)
        {
            NovelChapterTransitionModel transition = chapterTransitions[i];

            NovelTransitionNodeView nodeView = new NovelTransitionNodeView(transition);
            Rect savedPosition = GetSavedNodeTransitionPosition(transition, i);
            nodeView.SetPosition(savedPosition);

            AddElement(nodeView);

            string path = AssetDatabase.GetAssetPath(transition);
            nodeTransitionViews[path] = nodeView;
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

    private List<NovelChapterTransitionModel> LoadAllNovelTransitions()
    {
        List<NovelChapterTransitionModel> result = new();

        string[] transitionGuids = AssetDatabase.FindAssets("t:NovelChapterTransitionModel");

        foreach (string guid in transitionGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NovelChapterTransitionModel item = AssetDatabase.LoadAssetAtPath<NovelChapterTransitionModel>(path);

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
                CreateTransitionLinkFromSerializedField(sourceNode, "chapterTransitionModel");
            }
            else if (sourceModel is NovelItemChoiceableModel)
            {
                CreateLinkFromSerializedField(sourceNode, "firstChoiceModel");
                CreateLinkFromSerializedField(sourceNode, "secondChoiceModel");
                CreateLinkFromSerializedField(sourceNode, "thirdChoiceModel");
            }
        }

        foreach (NovelTransitionNodeView sourceNode in nodeTransitionViews.Values)
        {
            CreateLinkFromSerializedField(sourceNode, "startNovelModel");
        }
    }

    private void CreateTransitionLinkFromSerializedField(NovelNodeView sourceNode, string fieldName)
    {
        NovelChapterTransitionModel targetModel = GetTransitionReference(sourceNode.Model, fieldName);

        if (targetModel == null)
            return;

        string targetPath = AssetDatabase.GetAssetPath(targetModel);

        if (string.IsNullOrEmpty(targetPath))
        {
            Debug.LogWarning($"Transition target has no asset path: {targetModel.name}");
            return;
        }

        if (!nodeTransitionViews.TryGetValue(targetPath, out NovelTransitionNodeView targetNode))
        {
            Debug.LogWarning($"Transition node not found in graph: {targetModel.name}");
            return;
        }

        Port outputPort = sourceNode.GetOutputPort(fieldName);
        Port inputPort = targetNode.InputPort;

        if (outputPort == null || inputPort == null)
        {
            Debug.LogWarning($"Port missing for transition link: {sourceNode.Model.name}.{fieldName}");
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

        Debug.Log($"DRAW TRANSITION LINK: {sourceNode.Model.name}.{fieldName} -> {targetModel.name}");
    }

    private void SetNodeTransitionReference(
    AbstractNovelItemModel sourceModel,
    string fieldName,
    NovelChapterTransitionModel targetModel)
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
            Debug.LogError($"Field not found: {fieldName} on {sourceModel.name}");
            return;
        }

        Undo.RecordObject(sourceModel, "Update Node Transition Link");

        property.objectReferenceValue = targetModel;

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(sourceModel);
        AssetDatabase.SaveAssets();

        Debug.Log($"SAVED NODE -> TRANSITION LINK: {sourceModel.name}.{fieldName} -> {(targetModel != null ? targetModel.name : "null")}");
    }

    private NovelChapterTransitionModel GetTransitionReference(AbstractNovelItemModel sourceModel, string fieldName)
    {
        SerializedObject serializedObject = new SerializedObject(sourceModel);
        serializedObject.Update();

        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property == null)
            return null;

        return property.objectReferenceValue as NovelChapterTransitionModel;
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

    private void CreateLinkFromSerializedField(NovelTransitionNodeView sourceNode, string fieldName)
    {
        AbstractNovelItemModel targetModel = GetTransitionTargetReference(sourceNode.Model, fieldName);

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
                else if (element is NovelTransitionNodeView movedTransitionNode)
                {
                    SaveNodeTransitionPosition(movedTransitionNode);
                }
            }
        }

        if (graphViewChange.edgesToCreate != null)
        {
            foreach (Edge edge in graphViewChange.edgesToCreate)
            {
                string fieldName = edge.output.userData as string;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                if (edge.output?.node is NovelNodeView sourceNode &&
                    edge.input?.node is NovelNodeView targetNode)
                {
                    SetModelReference(sourceNode.Model, fieldName, targetNode.Model);
                }
                else if (edge.output?.node is NovelTransitionNodeView transitionSourceNode &&
                         edge.input?.node is NovelNodeView transitionTargetNode)
                {
                    SetTransitionModelReference(
                        transitionSourceNode.Model,
                        fieldName,
                        transitionTargetNode.Model
                    );
                }
                else if (edge.output?.node is NovelNodeView sourceNodeToTransition &&
                         edge.input?.node is NovelTransitionNodeView transitionTargetNode2)
                {
                    SetNodeTransitionReference(
                        sourceNodeToTransition.Model,
                        fieldName,
                        transitionTargetNode2.Model
                    );
                }
            }
        }

        if (graphViewChange.elementsToRemove != null)
        {
            foreach (GraphElement element in graphViewChange.elementsToRemove)
            {
                if (element is not Edge edge)
                    continue;

                string fieldName = edge.output?.userData as string;

                if (string.IsNullOrEmpty(fieldName))
                    continue;

                // Novel Node -> Novel Node
                if (edge.output?.node is NovelNodeView sourceNode &&
                    edge.input?.node is NovelNodeView targetNode)
                {
                    AbstractNovelItemModel currentTarget =
                        GetModelReference(sourceNode.Model, fieldName);

                    if (currentTarget == targetNode.Model)
                    {
                        SetModelReference(sourceNode.Model, fieldName, null);
                    }

                    continue;
                }

                // Novel Node -> Transition Node
                if (edge.output?.node is NovelNodeView sourceNodeToTransition &&
                    edge.input?.node is NovelTransitionNodeView transitionTargetNode)
                {
                    NovelChapterTransitionModel currentTarget =
                        GetTransitionReference(sourceNodeToTransition.Model, fieldName);

                    if (currentTarget == transitionTargetNode.Model)
                    {
                        SetNodeTransitionReference(sourceNodeToTransition.Model, fieldName, null);
                    }

                    continue;
                }

                // Transition Node -> Novel Node
                if (edge.output?.node is NovelTransitionNodeView transitionSourceNode &&
                    edge.input?.node is NovelNodeView transitionToNovelTargetNode)
                {
                    AbstractNovelItemModel currentTarget =
                        GetTransitionTargetReference(transitionSourceNode.Model, fieldName);

                    if (currentTarget == transitionToNovelTargetNode.Model)
                    {
                        SetTransitionModelReference(transitionSourceNode.Model, fieldName, null);
                    }

                    continue;
                }
            }
        }

        return graphViewChange;
    }

    private void SaveNodeTransitionPosition(NovelTransitionNodeView nodeView)
    {
        if (nodeView == null || nodeView.Model == null)
            return;

        Rect position = nodeView.GetPosition();
        string key = GetNodeTransitionPositionKey(nodeView.Model);

        EditorPrefs.SetFloat(key + "_x", position.x);
        EditorPrefs.SetFloat(key + "_y", position.y);

        Debug.Log($"SAVED TRANSITION POSITION: {nodeView.Model.name} -> {position.x}, {position.y}");
    }

    private string GetNodePositionKey(AbstractNovelItemModel model)
    {
        string assetPath = AssetDatabase.GetAssetPath(model);
        return $"NovelGraph_NodePosition_{assetPath}";
    }

    private string GetNodeTransitionPositionKey(NovelChapterTransitionModel model)
    {
        string assetPath = AssetDatabase.GetAssetPath(model);
        return $"NovelGraph_NodeTransitionPosition_{assetPath}";
    }

    private void SetTransitionModelReference(
    NovelChapterTransitionModel sourceModel,
    string fieldName,
    AbstractNovelItemModel targetModel)
    {
        if (sourceModel == null)
        {
            Debug.LogError("Transition source model is null.");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(sourceModel);
        serializedObject.Update();

        SerializedProperty property = serializedObject.FindProperty(fieldName);

        if (property == null)
        {
            Debug.LogError($"Field not found: {fieldName} on {sourceModel.name}");
            return;
        }

        Undo.RecordObject(sourceModel, "Update Transition Link");

        property.objectReferenceValue = targetModel;

        serializedObject.ApplyModifiedProperties();

        EditorUtility.SetDirty(sourceModel);
        AssetDatabase.SaveAssets();

        Debug.Log($"SAVED TRANSITION LINK: {sourceModel.name}.{fieldName} -> {(targetModel != null ? targetModel.name : "null")}");
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

    private Rect GetSavedNodeTransitionPosition(NovelChapterTransitionModel model, int index)
    {
        string key = GetNodeTransitionPositionKey(model);

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

    private AbstractNovelItemModel GetTransitionTargetReference(NovelChapterTransitionModel sourceModel, string fieldName)
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