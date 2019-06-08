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

    [SerializeField, AssetList(Path = "Resources/Themes"), InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
    private Theme theme;

    [HideInInspector]
    public float spacing = 0.8f;

    [SerializeField, FoldoutGroup("References")]
    private Tile tilePrefab;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI GoalText;

    [SerializeField, FoldoutGroup("References")]
    private Transform tileContainer;

    public Tile[,] tiles;

    [HideInInspector]
    public int changeCount;


    [SerializeField, FoldoutGroup("References")]
    private SpriteRenderer background;

    public void RandomTheme()
    {
        var themeArray = Resources.LoadAll<Theme>("Themes");
        theme = themeArray[Random.Range(0, themeArray.Length)];
        UpdateTheme();
    }

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
                tile.Theme = theme;
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
//            
//            do randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
//            while (randomTile.type != Tile.TileType.Number && randomTile.ToBeChanged);
//            randomTile.MarkToBeChanged();
        }
    }
}