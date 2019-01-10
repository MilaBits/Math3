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
            tile.SetValue(GameRules.TileValues[Random.Range(0, GameRules.TileValues.Count)]);
        }

        EnsureSingleOperators();
    }

    private void EnsureSingleOperators() {
        bool hasPlus = false;
        bool hasMinus = false;
        bool hasMultiply = false;
        bool hasDivide = false;

        foreach (Tile tile in tiles) {
            if (tile.GetValue() == "+") {
                if (!hasPlus) {
                    hasPlus = true;
                    continue;
                }

                tile.SetValue(Random.Range(1, 9).ToString());
            }

            if (tile.GetValue() == "-") {
                if (!hasMinus) {
                    hasMinus = true;
                    continue;
                }

                tile.SetValue(Random.Range(1, 9).ToString());
            }

            if (tile.GetValue() == "*") {
                if (!hasMultiply) {
                    hasMultiply = true;
                    continue;
                }

                tile.SetValue(Random.Range(1, 9).ToString());
            }

            if (tile.GetValue() == "/") {
                if (!hasDivide) {
                    hasDivide = true;
                    continue;
                }

                tile.SetValue(Random.Range(1, 9).ToString());
            }
        }

        while (!hasPlus) {
            Tile tile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            if (tile.type == Tile.TileType.Number) {
                tile.SetValue("+");
                break;
            }
        }

        while (!hasMinus) {
            Tile tile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            if (tile.type == Tile.TileType.Number) {
                tile.SetValue("-");
                break;
            }
        }

        while (!hasMultiply) {
            Tile tile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            if (tile.type == Tile.TileType.Number) {
                tile.SetValue("x");
                break;
            }
        }

        while (!hasDivide) {
            Tile tile = tiles[Random.Range(0, dimensions.x), Random.Range(0, dimensions.y)];
            if (tile.type == Tile.TileType.Number) {
                tile.SetValue("/");
                break;
            }
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
}