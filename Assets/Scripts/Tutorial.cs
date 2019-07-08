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

    [SerializeField]
    private TapAnimation tapAnimation;

    [SerializeField, ListDrawerSettings(NumberOfItemsPerPage = 5)]
    private List<string> gridValues;

    private int currentMessage = 0;

    private bool locked = true;

    public bool overTutorial;

    [SerializeField]
    private int tutorialTarget;

    public void setOverTutorial(bool value)
    {
        overTutorial = value;
    }

    private void Start()
    {
        StartCoroutine(DelayedStart());

        text.text = tutorialMessages[0].Text;
        progressText.text = $"1/{tutorialMessages.Count}";

        Resources.LoadAll<Settings>("Settings").First().WatchedTutorial = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && tutorialMessages[currentMessage].WaitForInput && overTutorial)
        {
            NextMessage();
        }
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        grid.SetGridValues(gridValues);

        grid.GameRules.CurrentAnswer = tutorialTarget;
        grid.GameRules.NextAnswerEvent.Invoke();

        foreach (TutorialMessage tutorialMessage in tutorialMessages)
        {
            tutorialMessage.target.Transform = GetClosestTile(tutorialMessage.target.Position).transform;
        }

        locked = false;
        NextMessage();
    }

    private void LoadTutorialMessage()
    {
        locked = true;

        progressText.text = $"{currentMessage + 1}/{tutorialMessages.Count}";
        UpdateHighlight(tutorialMessages[currentMessage]);

        currentMessage++;
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

    private IEnumerator FillBar()
    {
        float start = progressBar.fillAmount;
        for (float progress = 0; progress < highlightSpeed; progress += Time.deltaTime)
        {
            progressBar.fillAmount =
                Mathf.Lerp(start, (float) currentMessage / tutorialMessages.Count, progress / highlightSpeed);
            yield return null;
        }

        progressBar.fillAmount = (float) currentMessage / tutorialMessages.Count;
    }

    private void UpdateHighlight(TutorialMessage message)
    {
        text.text = message.Text;

        StartCoroutine(FillBar());
        Vector3 position = message.target.OffGrid
            ? (Vector3) message.target.Position
            : message.target.Transform.position;

        StartCoroutine(DoClicks(message));
        StartCoroutine(MoveHighlight(
            position,
            new Vector2(
                message.target.Size.x * blockSize + paddingSize,
                message.target.Size.y * blockSize + paddingSize),
            highlightSpeed));
    }

    private IEnumerator DoClicks(TutorialMessage message)
    {
        foreach (var click in message.Clicks)
        {
            clicker.Click(click);
            tapAnimation.Play(click);
            yield return new WaitForSeconds(message.ClickInterval);
        }
    }

    public void NextMessage()
    {
        if (!locked) LoadTutorialMessage();
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

        locked = false;
        if (currentMessage >= tutorialMessages.Count) Destroy(gameObject);
    }
}