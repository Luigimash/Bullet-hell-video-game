using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndMenu : MonoBehaviour
{
	public Button mainMenu;
	public Button replayGame;
	public GameObject scoreStore;
	private GameObject[] objs;
	public Text scoreTxt;
	
    // Start is called before the first frame update
    void Start()
    {
		scoreStore = GameObject.Find("scoreStore");
		
		scoreTxt.text = ((int)scoreStore.GetComponent<ScoreStore>().scoreValue).ToString("D7");
		
        replayGame.onClick.AddListener(Restart);
		mainMenu.onClick.AddListener(MainMenu);
		
		SceneManager.UnloadSceneAsync(1);
    }

	void Restart()
	{
		SceneManager.LoadScene(1);
		Debug.Log("Loading scene 1");
	}
	
	void MainMenu()
	{
		SceneManager.LoadScene(0);
		Debug.Log("Loading scene 1");
	}	
}
