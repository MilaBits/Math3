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
    public float duration;
}