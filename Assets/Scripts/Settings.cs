using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu]
public class Settings : ScriptableObject
{
    [AssetList(Path = "Resources/Themes"), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    public Theme Theme;

    public bool WatchedTutorial;
}