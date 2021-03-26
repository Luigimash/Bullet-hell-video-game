using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public Button startGame;
	public Button exit;
	
    // Start is called before the first frame update
    void Start()
    {
        startGame.onClick.AddListener(initiateGame);
		exit.onClick.AddListener(exitGame);
    }

	void initiateGame()
	{
		SceneManager.LoadScene(1);
		Debug.Log("Loading scene 1");
	}
	
	void exitGame()
	{
		Application.Quit();
		Debug.Log("Quit");
	}	
}
