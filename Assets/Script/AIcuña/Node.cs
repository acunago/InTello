using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class Node
{
    public string id;
    public string title;
    public Rect rect;

    [NonSerialized] public bool isDragged;
    [NonSerialized] public bool isSelected;

    [NonSerialized] public GUIStyle style;
    [NonSerialized] public GUIStyle defaultNodeStyle;
    [NonSerialized] public GUIStyle selectedNodeStyle;

    [NonSerialized] public Action<Node> OnRemoveNode;

    public Node(Vector2 position, float width, float height,
        GUIStyle nodeStyle, GUIStyle selectedStyle, Action<Node> OnClickRemoveNode, string id = null)
    {
        rect = new Rect(position.x, position.y, width, height);
        style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        OnRemoveNode = OnClickRemoveNode;

        this.id = id ?? Guid.NewGuid().ToString();
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public virtual void Draw()
    {
        GUI.Box(rect, title, style);
    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    private void OnClickRemoveNode()
    {
        OnRemoveNode?.Invoke(this);
    }
}
