using System;
using UnityEngine;

[Serializable]
public struct HighlightTarget
{
    public bool OffGrid;
    public Vector2 Position;
    public Transform Transform;
    public Vector2 Size;
}