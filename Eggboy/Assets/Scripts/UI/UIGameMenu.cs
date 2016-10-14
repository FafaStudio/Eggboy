using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIGameMenu : MonoBehaviour {

	private GameObject GameOverPanel;

	void Start () {
		GameOverPanel = GameObject.Find ("GameOverPanel").gameObject;
		GameOverPanel.SetActive (false);
	}

	public void retryButton(){
		GameManager.instance.restartGame ();
	}

	public void quitButton(){
		Application.Quit ();
	}

	public void launchGameOver(){
		GameOverPanel.SetActive (true);
	}
}
