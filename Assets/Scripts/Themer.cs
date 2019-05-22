using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Themer : MonoBehaviour
{
    [SerializeField]
    private Theme theme;

    [Space]
    [SerializeField]
    private TextMeshProUGUI Score;
    
    [SerializeField]
    private TileGrid grid;

    [SerializeField]
    private SpriteRenderer background;
    
//    private void Start()
//    {
//        Score.color = theme.TextColor;
//        foreach (Tile tile in grid.tiles)
//        {
//            
//            tile.DefaultColor = theme.TileColor;
//            tile.Operator1Color = theme.Operator1Color;
//            tile.Operator2Color = theme.Operator2Color;
//            
//            switch (tile.type)
//            {
//                case Tile.TileType.Number:
//                    tile.SetColor(theme.TileColor);
//                    break;
//                case Tile.TileType.Operator:
//                    tile.SetColor(theme.Operator1Color);
//                    break;
//            }
//        }
//        
//        background.color = theme.BackgroundColor;
//        
//    }
}