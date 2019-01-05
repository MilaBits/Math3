using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TileGrid : MonoBehaviour {
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

    public GridRules GridRules;

    void Start() {
        GenerateGrid();

        GenerateValues();

        GoalText.text = GridRules.Answers[Random.Range(0, GridRules.Answers.Count - 1)].ToString();
    }

    public void GenerateValues() {
        foreach (var tile in tiles) {
            tile.SetValue(GridRules.TileValues[Random.Range(0, GridRules.TileValues.Count)]);
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

                tile.SetValue(Random.Range(0, 9).ToString());
            }

            if (tile.GetValue() == "-") {
                if (!hasMinus) {
                    hasMinus = true;
                    continue;
                }

                tile.SetValue(Random.Range(0, 9).ToString());
            }

            if (tile.GetValue() == "x") {
                if (!hasMultiply) {
                    hasMultiply = true;
                    continue;
                }

                tile.SetValue(Random.Range(0, 9).ToString());
            }

            if (tile.GetValue() == "/") {
                if (!hasDivide) {
                    hasDivide = true;
                    continue;
                }

                tile.SetValue(Random.Range(0, 9).ToString());
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
                tiles[x, y] = tile;

                tile.transform.localPosition = new Vector2(x * spacing, -y * spacing);
            }
        }
    }
}