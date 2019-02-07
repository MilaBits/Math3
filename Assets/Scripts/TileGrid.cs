using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileGrid : MonoBehaviour {
    public Vector2Int dimensions;

    [HideInInspector] public float spacing = 0.8f;

    [SerializeField] private Tile tilePrefab;

    [SerializeField] private TextMeshProUGUI GoalText;

    [SerializeField] private Transform tileContainer;

    public Tile[,] tiles;

    public GameRules GameRules;

    void Start() {
        GenerateGrid();

        GenerateValues();

        GameRules.NextAnswerEvent.AddListener(UpdateGoal);
        UpdateGoal();
    }

    public void UpdateGoal() {
        GoalText.text = GameRules.CurrentAnswer.ToString();
    }

    public void GenerateValues() {
        foreach (var tile in tiles) {
            tile.SetValue(GameRules.Values[Random.Range(0, GameRules.Values.Count)]);
        }

        EnsureSingleOperators();

        for (int i = 0; i < GameRules.ChangingTileCount; i++) {
            MarkRandomToBeChanged();
        }
    }

    private void EnsureSingleOperators() {
        foreach (string gameRulesOperator in GameRules.Operators) {
            Tile randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];

            while (randomTile.type == Tile.TileType.Operator) {
                randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            }

            randomTile.SetValue(gameRulesOperator);
        }
    }

    private void GenerateGrid() {
        tiles = new Tile[dimensions.x, dimensions.y];
        for (int x = 0; x < dimensions.x; x++) {
            for (int y = 0; y < dimensions.y; y++) {
                Tile tile = Instantiate(tilePrefab, tileContainer);
                tile.GameRules = GameRules;
                tiles[x, y] = tile;

                tile.transform.localPosition = new Vector2(x * spacing, -y * spacing);
            }
        }
    }

    public void MarkRandomToBeChanged() {
        Tile randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];

        while (randomTile.ToBeChanged) {
            randomTile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
        }

        if (randomTile.type == Tile.TileType.Number) {
            randomTile.MarkToBeChanged();
            return;
        }

        MarkRandomToBeChanged();
    }
}