using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class TutorialMessage : ScriptableObject
{
    [FoldoutGroup("$name")]
    [LabelWidth(35)]
    public string Text;

    [FoldoutGroup("$name")]
    public HighlightTarget target;

    [FoldoutGroup("$name")]
    public bool WaitForInput = true;

    [FoldoutGroup("$name"), HideIf("WaitForInput")]
    public float duration;

    [FoldoutGroup("$name")]
    public float ClickInterval;

    [FoldoutGroup("$name")]
    public List<Vector2> Clicks;
}