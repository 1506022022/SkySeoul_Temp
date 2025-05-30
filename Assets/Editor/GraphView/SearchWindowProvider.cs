using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class SearchWindowProvider : ScriptableObject, ISearchWindowProvider
{
    private BaseGraphView _graphView;
    private EditorWindow _editorWindow;

    public void Init(BaseGraphView graphView, EditorWindow editorWindow)
    {
        _graphView = graphView;
        _editorWindow = editorWindow;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Node")),
            new SearchTreeEntry(new GUIContent("BranchPath"))
            {
                level = 1,
                userData = nameof(BranchPathNode)
            },
            new SearchTreeEntry(new GUIContent("Path"))
            {
                level = 1,
                userData = nameof(PathNode)
            },
            new SearchTreeEntry(new GUIContent("Condition"))
            {
                level = 1,
                userData = nameof(ConditionNode)
            }
        };

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
    {
        Vector2 mousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(_editorWindow.rootVisualElement.parent,
            context.screenMousePosition - _editorWindow.position.position);

        Vector2 graphMousePosition = _graphView.contentViewContainer.WorldToLocal(mousePosition);

        _graphView.AddNode(graphMousePosition, (string)entry.userData);
        return true;
    }
}
