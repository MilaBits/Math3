using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    [InlineEditor]
    public GameRules GameRules;
    
    [SerializeField, AssetList(Path = "Resources/Themes"), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    private Theme theme;

    [Space]
    public Vector2Int dimensions;

    [HideInInspector]
    public float spacing = 0.8f;

    [SerializeField]
    private Tile tilePrefab;

    [SerializeField]
    private TextMeshProUGUI GoalText;

    [SerializeField]
    private Transform tileContainer;

    public Tile[,] tiles;


    [SerializeField]
    private SpriteRenderer background;

    public void RandomTheme()
    {
        var themeArray = Resources.LoadAll<Theme>("Themes");
        theme = themeArray[Random.Range(0, themeArray.Length)];
        UpdateTheme();
    }

    [Button, PropertyOrder(-1)]
    public void UpdateTheme()
    {
        foreach (Tile tile in tiles)
        {
            tile.UpdateTheme(theme);
        }

        background.color = theme.BackgroundColor;
        GoalText.color = theme.TextColor;
    }

    void Start()
    {
        GenerateGrid();

        GenerateValues();

	    GameRules.Instantiate(this);
        GameRules.NextAnswerEvent.AddListener(UpdateGoal);
        UpdateGoal();

        background.color = theme.BackgroundColor;
        GoalText.color = theme.TextColor;
    }

    public void UpdateGoal()
    {
        GoalText.text = GameRules.CurrentAnswer.ToString();
    }

    public void GenerateValues()
    {
        foreach (var tile in tiles)
        {
            tile.SetValue(GameRules.Values[Random.Range(0, GameRules.Values.Count)]);
        }

        EnsureSingleOperators();

        for (int i = 0; i < GameRules.ChangingTileCount; i++)
        {
            MarkRandomToBeChanged();
        }
    }

    private void EnsureSingleOperators()
    {
        foreach (string gameRulesOperator in GameRules.Operators)
        {
            Tile randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];

            while (randomTile.type == (Tile.TileType.Operator1 | Tile.TileType.Operator2))
            {
                randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            }

            randomTile.SetValue(gameRulesOperator);
        }
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
                tile.Theme = theme;
                tiles[x, y] = tile;

                tile.transform.localPosition = new Vector2(x * spacing, -y * spacing);
            }
        }
    }

    public void MarkRandomToBeChanged()
    {
        Tile randomTile;
        do
        {
            randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
        } while (randomTile.ToBeChanged);

        if (randomTile.type == Tile.TileType.Number)
        {
            randomTile.MarkToBeChanged();
            return;
        }

        MarkRandomToBeChanged();
    }
}