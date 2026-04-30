using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class NovelGraphWindow : EditorWindow
{
    private NovelGraphView graphView;

    [MenuItem("Tools/Novel/Novel Graph")]
    public static void Open()
    {
        NovelGraphWindow window = GetWindow<NovelGraphWindow>();
        window.titleContent = new GUIContent("Novel Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
    }

    private void OnDisable()
    {
        if (graphView != null)
        {
            rootVisualElement.Remove(graphView);
        }
    }

    private void ConstructGraphView()
    {
        rootVisualElement.Clear();

        VisualElement toolbar = new VisualElement();
        toolbar.style.height = 30;
        toolbar.style.flexDirection = FlexDirection.Row;
        toolbar.style.alignItems = Align.Center;
        toolbar.style.paddingLeft = 6;
        toolbar.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f);

        Button refreshButton = new Button(() =>
        {
            graphView.LoadExistingNovelItems();
        })
        {
            text = "Refresh Graph"
        };

        toolbar.Add(refreshButton);
        rootVisualElement.Add(toolbar);

        graphView = new NovelGraphView
        {
            name = "Novel Graph"
        };

        graphView.style.flexGrow = 1;
        rootVisualElement.Add(graphView);

        graphView.LoadExistingNovelItems();
    }
}