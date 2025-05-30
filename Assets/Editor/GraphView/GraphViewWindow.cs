using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using System.Linq;

public class GraphViewWindow : EditorWindow
{
    private string currentFilePath;
    private Vector2 mousePosition;
    private BaseGraphView graphView;
    private SearchWindowProvider searchWindowProvider;

    [MenuItem("Window/JsonGraphView")]
    public static void ShowGraphView()
    {
        var window = GetWindow<GraphViewWindow>("JsonGraphView");
        window.minSize = new Vector2(500, 500);
    }
    public void CreateGUI()
    {
        graphView = CreateGraph();
        rootVisualElement.Add(graphView);

        Toolbar toolbar = GetToolbar();
        rootVisualElement.Add(toolbar);

        CreateSearchWindow();
        graphView.ResetGraph();
    }
    private BaseGraphView CreateGraph()
    {
        var graphView = new BaseGraphView(this)
        {
            name = "JsonGraphView"
        };
        graphView.StretchToParentSize();
        graphView.AddManipulator(new ContentZoomer());
        graphView.SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        graphView.AddManipulator(new ContentDragger());
        graphView.AddManipulator(new SelectionDragger());
        graphView.AddManipulator(new RectangleSelector());
        graphView.AddManipulator(new ContextualMenuManipulator(OnContextMenu));
        graphView.Insert(0, new GridBackground());
        return graphView;
    }
    private void OnContextMenu(ContextualMenuPopulateEvent evt)
    {
        mousePosition = evt.mousePosition;
        evt.menu.AppendAction("Add Node", OpenSearchWindow);
    }
    private void CreateSearchWindow()
    {
        searchWindowProvider = ScriptableObject.CreateInstance<SearchWindowProvider>();
        searchWindowProvider.Init(graphView, this);
    }
    private void OpenSearchWindow(DropdownMenuAction action)
    {
        SearchWindow.Open(new SearchWindowContext(mousePosition), searchWindowProvider);
    }
    private Toolbar GetToolbar()
    {
        var toolbar = new Toolbar();

        var saveButton = new ToolbarButton(() => SaveGraph(currentFilePath)) { text = "저장" };
        toolbar.Add(saveButton);

        var saveAsButton = new ToolbarButton(() => SaveGraphAs()) { text = "다른 이름으로 저장" };
        toolbar.Add(saveAsButton);

        var loadButton = new ToolbarButton(() => LoadGraph()) { text = "불러오기" };
        toolbar.Add(loadButton);

        var funcButton = new ToolbarButton(() => TestFunc()) { text = "테스트" };
        toolbar.Add(funcButton);
        return toolbar;
    }
    private void TestFunc()
    {
        var baseType = typeof(BaseNode);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => baseType.IsAssignableFrom(t));
        foreach (var type in types)
        {
            Debug.Log(type.Name);
        }

    }
    private void SaveGraph(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            SaveGraphAs();
            return;
        }

        graphView.SaveGraph(path);
    }
    private void SaveGraphAs()
    {
        string path = EditorUtility.SaveFilePanel("그래프 저장", Application.dataPath, "MyGraph", "json");
        if (!string.IsNullOrEmpty(path))
        {
            currentFilePath = path;
            SaveGraph(path);
        }
    }
    private void LoadGraph()
    {
        string path = EditorUtility.OpenFilePanel("그래프 불러오기", Application.dataPath, "json");
        if (!string.IsNullOrEmpty(path))
        {
            currentFilePath = path;
            graphView.LoadGraph(path);
        }
    }
}
