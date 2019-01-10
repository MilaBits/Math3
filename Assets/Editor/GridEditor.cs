using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileGrid))]
public class GridEditor : Editor {
    [DrawGizmo(GizmoType.Selected | GizmoType.Active | GizmoType.NonSelected)]
    private void OnSceneGUI() {
        TileGrid tileGrid = (TileGrid) target;

        Vector2 pos = new Vector2(tileGrid.transform.position.x, tileGrid.transform.position.y);

        Vector2 topLeft =
            pos + new Vector2(
                -tileGrid.spacing / 2,
                +tileGrid.spacing / 2);
        Vector2 topRight =
            pos + new Vector2(
                tileGrid.spacing * (tileGrid.dimensions.x - 1) + tileGrid.spacing / 2,
                0 + tileGrid.spacing / 2);
        Vector2 bottomLeft =
            pos + new Vector2(
                -tileGrid.spacing / 2,
                tileGrid.spacing * (-tileGrid.dimensions.y + 1) - tileGrid.spacing / 2);
        Vector2 bottomRight =
            pos + new Vector2(
                tileGrid.spacing * (tileGrid.dimensions.x - 1) + tileGrid.spacing / 2,
                tileGrid.spacing * (-tileGrid.dimensions.y + 1) - tileGrid.spacing / 2);

        Handles.DrawLine(topLeft, topRight);
        Handles.DrawLine(topRight, bottomRight);
        Handles.DrawLine(bottomRight, bottomLeft);
        Handles.DrawLine(bottomLeft, topLeft);
    }

    public override void OnInspectorGUI() {
        TileGrid tileGrid = (TileGrid) target;
        if (GUILayout.Button("Re-generate values")) {
            tileGrid.GenerateValues();
        }

        DrawDefaultInspector();
    }

    private void OnValidate() {
        TileGrid tileGrid = (TileGrid) target;
    }
}