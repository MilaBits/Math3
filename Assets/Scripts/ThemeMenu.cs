using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThemeMenu : MonoBehaviour
{
    [SerializeField]
    private ThemeItem ThemeItemPrefab;

    void Start()
    {
        Theme[] themes = Resources.LoadAll<Theme>("Themes");

        foreach (Theme theme in themes)
        {
            var item = Instantiate(ThemeItemPrefab, transform);
            item.Initialize(theme);
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}