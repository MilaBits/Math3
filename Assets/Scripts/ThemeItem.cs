using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ThemeItem : MonoBehaviour
{
    private Theme theme;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI title;
    
    [SerializeField, FoldoutGroup("References")]
    private Image background;

    [SerializeField, FoldoutGroup("References")]
    private Image baseImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI baseText;

    [SerializeField, FoldoutGroup("References")]
    private Image solveImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI solveText;

    [SerializeField, FoldoutGroup("References")]
    private Image changeImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI changeText;

    [SerializeField, FoldoutGroup("References")]
    private Image plusImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI plusText;

    [SerializeField, FoldoutGroup("References")]
    private Image minusImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI minusText;

    [SerializeField, FoldoutGroup("References")]
    private Image multiplyImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI multiplyText;

    [SerializeField, FoldoutGroup("References")]
    private Image divideImage;

    [SerializeField, FoldoutGroup("References")]
    private TextMeshProUGUI divideText;

    public void Initialize(Theme theme)
    {
        this.theme = theme;
        
        title.text = theme.name;
        plusText.text = GameRules.Plus;
        minusText.text = GameRules.Minus;
        multiplyText.text = GameRules.Multiply;
        divideText.text = GameRules.Divide;
        
        background.color = theme.BackgroundColor;

        baseImage.color = theme.TileColor;
        changeImage.color = theme.ChangeColor;
        solveImage.color = theme.SolutionColor;
        plusImage.color = theme.Operator1Color;
        minusImage.color = theme.Operator1Color;
        multiplyImage.color = theme.Operator2Color;
        divideImage.color = theme.Operator2Color;
        

        title.color = theme.TextColor;

        baseText.color= theme.TileTextColor;
        changeText.color= theme.TileTextColor;
        solveText.color= theme.TileTextColor;
        plusText.color= theme.TileTextColor;
        minusText.color= theme.TileTextColor;
        multiplyText.color= theme.TileTextColor;
        divideText.color= theme.TileTextColor;
    }

    public void SetTheme()
    {
        PlayerPrefs.SetString("Theme", theme.name);

        SceneManager.LoadScene("MainMenu");
    }
}