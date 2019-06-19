using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeItem : MonoBehaviour
{
    private Theme theme;

    [SerializeField]
    private TextMeshProUGUI title;

    [SerializeField]
    private Transform colorPanel;

    public void Initialize(Theme theme)
    {
        this.theme = theme;

        title.text = theme.name;
    }

    public void SetTheme()
    {
        TileGrid.theme = theme;
        SceneManager.LoadScene("MainMenu");
    }
}