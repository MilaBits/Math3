using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class Theme : ScriptableObject
{
    [Header("Tile Backgrounds")]
    [LabelText("Number")]
    [OnValueChanged("InvokeChanged")]
    public Color TileColor;

    [LabelText("Operator 1")]
    [OnValueChanged("InvokeChanged")]
    public Color Operator1Color;

    [LabelText("Operator 2")]
    [OnValueChanged("InvokeChanged")]
    public Color Operator2Color;

    [Header("Text")]
    [LabelText("UI Text")]
    [OnValueChanged("InvokeChanged")]
    public Color TextColor;

    [LabelText("Tile Text")]
    [OnValueChanged("InvokeChanged")]
    public Color TileTextColor;

    [Header("Events")]
    [LabelText("Solved")]
    [OnValueChanged("InvokeChanged")]
    public Color SolutionColor;

    [LabelText("Changing")]
    [OnValueChanged("InvokeChanged")]
    public Color ChangeColor;

    [Space, Space]
    [LabelText("Background")]
    [OnValueChanged("InvokeChanged")]
    public Color BackgroundColor;

    [OnValueChanged("InvokeChanged")]
    public Color GridColor;

    private void InvokeChanged()
    {
        Changed.Invoke();
    }

    [HideInInspector]
    public UnityEvent Changed;
}