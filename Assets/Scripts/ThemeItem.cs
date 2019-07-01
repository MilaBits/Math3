using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    private Settings settings;

    public void Initialize(Theme theme)
    {
        this.theme = theme;

        title.text = theme.name;
    }

    public void SetTheme()
    {
        Resources.LoadAll<Settings>("Settings").First().Theme = theme;
        SceneManager.LoadScene("MainMenu");
    }
}