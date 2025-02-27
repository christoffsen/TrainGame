using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewGameButton : MonoBehaviour
{
	public void StartNewGame ()
	{
		SceneManager.LoadSceneAsync(1);
	}

	public void ExitGame()
	{
		Application.Quit();
	}
}
