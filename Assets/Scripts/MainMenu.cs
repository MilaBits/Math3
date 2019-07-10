using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[InlineEditor]
	public Theme theme;

	private void Awake()
	{
		PlayerPrefs.DeleteAll();
		if (PlayerPrefs.GetString("Theme").IsNullOrWhitespace()) PlayerPrefs.SetString("Theme", "Default");
	}

	public void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}
}
