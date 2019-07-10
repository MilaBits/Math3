using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void Switch(string scene)
    {
        if (scene == "GameScene" && PlayerPrefs.GetInt("WatchedTutorial") == 0)
        {
            SceneManager.LoadScene("TutorialScene");
            return;
        }

        SceneManager.LoadScene(scene);
    }
}