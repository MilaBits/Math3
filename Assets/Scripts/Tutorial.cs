using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField, BoxGroup("Highlight Settings")]
    private RectTransform highlight;

    [SerializeField, BoxGroup("Highlight Settings")]
    private float blockSize;

    [SerializeField, BoxGroup("Highlight Settings")]
    private float paddingSize;

    [SerializeField, BoxGroup("Highlight Settings")]
    private float highlightSpeed;

    [SerializeField, InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    private List<TutorialMessage> tutorialMessages = new List<TutorialMessage>();

    [SerializeField]
    private TextMeshPro text;

    [SerializeField]
    private TextMeshProUGUI progressText;

    [SerializeField]
    private Image progressBar;

    [SerializeField, BoxGroup("Debug")]
    private Vector2Int highlightSize;

    [SerializeField, BoxGroup("Debug")]
    private Tile centerTile;

    [SerializeField]
    private Clicker clicker;

    [SerializeField]
    private TileGrid grid;

    private void Start()
    {
        StartCoroutine(DelayedStart());

        text.text = tutorialMessages[0].Text;
        progressText.text = $"1/{tutorialMessages.Count}";

        Resources.LoadAll<Settings>("Settings").First().WatchedTutorial = true;
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);

        foreach (TutorialMessage tutorialMessage in tutorialMessages)
        {
            tutorialMessage.target.Transform = GetClosestTile(tutorialMessage.target.Position).transform;
        }

        StartCoroutine(PlayTutorial(0));
    }

    private IEnumerator PlayTutorial(int index)
    {
        if (tutorialMessages.Count <= index)
        {
            Destroy(gameObject);
            yield break;
        }


        progressText.text = $"{index + 1}/{tutorialMessages.Count}";
        UpdateHighlight(tutorialMessages[index]);
        yield return new WaitForSeconds(tutorialMessages[index].duration);
        StartCoroutine(PlayTutorial(index + 1));
    }

    private Tile GetClosestTile(Vector2 position)
    {
        // Get closest tile to click
        float closestDistance = 100f;
        Tile closestTile = grid.tiles[0, 0];
        foreach (Tile tile in grid.tiles)
        {
            float distance = Vector2.Distance(tile.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = tile;
            }
        }

        return closestTile;
    }

    private IEnumerator FillBar(float t)
    {
        for (float e = 0; e < t; e += Time.deltaTime)
        {
            progressBar.fillAmount = e / t;
            yield return null;
        }

        progressBar.fillAmount = 1;
    }

    private void UpdateHighlight(TutorialMessage message)
    {
        text.text = message.Text;

        StartCoroutine(MoveHighlight(
            message.target.Transform.position,
            new Vector2(
                message.target.Size.x * blockSize + paddingSize,
                message.target.Size.y * blockSize + paddingSize),
            highlightSpeed));
        StartCoroutine(FillBar(message.duration));
    }

    private IEnumerator MoveHighlight(Vector2 targetPosition, Vector2 targetSize, float t)
    {
        Vector2 startPos = highlight.position;
        Vector2 startSize = highlight.sizeDelta;

        for (float e = 0; e < t; e += Time.deltaTime)
        {
            highlight.position = Vector2.Lerp(startPos, targetPosition, e / t);
            highlight.sizeDelta = Vector2.Lerp(startSize, targetSize, e / t);

            yield return null;
        }

        highlight.position = targetPosition;
        highlight.sizeDelta = targetSize;
    }
}