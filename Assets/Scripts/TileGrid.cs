using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [BoxGroup("Grid Settings", false)]
    public Vector2Int dimensions;

    [InlineEditor, AssetList(Path = "Scripts/Persistent/GridRules")]
    public GameRules GameRules;

    public Theme Theme;

    [HideInInspector]
    public float spacing = 0.8f;

    [SerializeField, FoldoutGroup("References")]
    private Tile tilePrefab;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshPro GoalText;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshPro TimeText;

    [SerializeField, FoldoutGroup("References")]
    private SpriteRenderer arrows;

    [SerializeField, FoldoutGroup("References")]
    private SpriteRenderer gridlines;

    [SerializeField, FoldoutGroup("References")]
    private Transform tileContainer;

    [SerializeField, FoldoutGroup("References")]
    private AudioSource audioSource;

    [SerializeField]
    private ScorePopup popupPrefab;

    public Tile[,] tiles;

    [SerializeField]
    private AudioClip solvedSound;

    [HideInInspector]
    public int changeCount;

    [SerializeField, FoldoutGroup("References")]
    private SpriteRenderer background;

    private Transform popupTarget;

    [SerializeField]
    private bool manualGrid;

    [Button, ShowIf("manualGrid")]
    private void GenerateInEditor()
    {
        GenerateGrid();
        GenerateValues();
    }

    [Button, ShowIf("manualGrid")]
    private void Clear()
    {
        foreach (Transform child in tileContainer.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    private void Awake()
    {
        Theme = Resources.LoadAll<Settings>("Settings").First().Theme;
    }

    void Start()
    {
        changeCount = 0;

        GenerateGrid();

        GenerateValues();

        GameRules.Instantiate(this);
        GameRules.NextAnswerEvent.AddListener(UpdateGoal);
        UpdateGoal();

        arrows.color = Theme.GridColor;
        gridlines.color = Theme.GridColor;

        TimeText.color = Theme.TextColor;
        GoalText.color = Theme.TextColor;

        background.color = Theme.BackgroundColor;
    }

    public void UpdateGoal()
    {
        audioSource.PlayOneShot(solvedSound);

        GoalText.text = GameRules.CurrentAnswer.ToString();
    }

    public void GenerateValues()
    {
        foreach (var tile in tiles)
        {
            tile.SetValue(GameRules.TileValues[Random.Range(0, GameRules.TileValues.Count)]);
        }

        EnsureSingleOperators();
        MarkRandomToBeChanged(true);
    }

    private void EnsureSingleOperators()
    {
        if ((GameRules.ActiveOperators & GameRules.Operator.Plus) != 0) SetRandomTileOperator(GameRules.Plus);
        if ((GameRules.ActiveOperators & GameRules.Operator.Minus) != 0) SetRandomTileOperator(GameRules.Minus);
        if ((GameRules.ActiveOperators & GameRules.Operator.Multiply) != 0) SetRandomTileOperator(GameRules.Multiply);
        if ((GameRules.ActiveOperators & GameRules.Operator.Divide) != 0) SetRandomTileOperator(GameRules.Divide);
    }

    private void SetRandomTileOperator(string operatorText)
    {
        Tile randomTile;
        do randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
        while (randomTile.type != Tile.TileType.Number);
        randomTile.SetValue(operatorText);
    }

    private void GenerateGrid()
    {
        tiles = new Tile[dimensions.x, dimensions.y];
        for (int x = 0; x < dimensions.x; x++)
        {
            for (int y = 0; y < dimensions.y; y++)
            {
                Tile tile = Instantiate(tilePrefab, tileContainer);
                tile.GameRules = GameRules;
                tile.popupTarget = GoalText.transform.position;
                tiles[x, y] = tile;

                tile.transform.localPosition = new Vector2(x * spacing, -y * spacing);
            }
        }
    }

    public void MarkRandomToBeChanged(bool init)
    {
        int count = 1;

        if (init) count = GameRules.ChangingTileCount;

        for (int i = 0; i < count; i++)
        {
            if (changeCount >= GameRules.ChangingTileCount) return;
            var filteredList = tiles.FilterCast<Tile>().Where(t => t.type == Tile.TileType.Number && !t.ToBeChanged);
            filteredList.ElementAt(Random.Range(0, filteredList.Count())).MarkToBeChanged();
            changeCount++;
        }
    }
}