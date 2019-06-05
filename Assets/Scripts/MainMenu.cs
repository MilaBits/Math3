using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	[InlineEditor]
	public Theme theme;
	
	
	public void LoadScene(string scene)
	{
		SceneManager.LoadScene(scene);
	}
}
