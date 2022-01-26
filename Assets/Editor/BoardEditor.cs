using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Board))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Board board = (Board)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Build"))
        {
            board.Clear();
            board.Build();
        }
    }
}
