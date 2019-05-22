using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class Theme : ScriptableObject
{
    [Header("Tile Backgrounds")]
    [LabelText("Number")]
    public Color TileColor;

    [LabelText("Operator 1")]
    public Color Operator1Color;

    [LabelText("Operator 2")]
    public Color Operator2Color;

    [Header("Text")]
    [LabelText("UI Text")]
    public Color TextColor;

    [LabelText("Tile Text")]
    public Color TileTextColor;

    [Header("Events")]
    [LabelText("Solved")]
    public Color SolutionColor;

    [LabelText("Changing")]
    public Color ChangeColor;

    [Space,Space]
    [LabelText("Background")]
    public Color BackgroundColor;
}