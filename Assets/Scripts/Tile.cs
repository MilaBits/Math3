using System.Collections;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour {
    [SerializeField]
    private TextMeshProUGUI textMesh;

    public TileType type;

    public Color DefaultColor;
    public Color Operator1Color;
    public Color Operator2Color;
    private SpriteRenderer renderer;


    private const string Plus = "+";
    private const string Minus = "-";
    private const string Multiply = "x";
    private const string Divide = "/";

    [SerializeField]
    private string value;

    public void SetValue(string text) {
        value = text;
        textMesh.text = value;

        // Dirty way
        switch (value) {
            case Plus:
            case Minus:
                type = TileType.Operator;
                renderer.color = Operator1Color;
                break;
            case Multiply:
            case Divide:
                type = TileType.Operator;
                renderer.color = Operator2Color;
                break;
            default:
                type = TileType.Number;
                renderer.color = DefaultColor;
                break;
        }
    }

    private void Awake() {
        renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public string GetValue() {
        return value;
    }

    public void Slide(Vector3 target, float time) {
        StartCoroutine(SlideCoroutine(target, time));
    }

    private IEnumerator SlideCoroutine(Vector3 target, float time) {
        float elapsedTime = 0;

        while (elapsedTime < time) {
            transform.position = Vector3.Lerp(transform.position, target, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    public enum TileType {
        Number,
        Operator
    }
}