using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void Switch(string scene)
    {
        if (scene == "GameScene" && !Resources.LoadAll<Settings>("Settings").First().WatchedTutorial)
        {
            SceneManager.LoadScene("TutorialScene");
            return;
        }

        SceneManager.LoadScene(scene);
    }
}