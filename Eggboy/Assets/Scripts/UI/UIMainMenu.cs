using UnityEngine;
using System.Collections;

public class UIMainMenu : MonoBehaviour {

	private GameObject tipsPanel;
	private bool launchingGameReady = false;

	private GameObject mainPanel;

	void Start () {
		tipsPanel = GameObject.Find ("TipsPanel");
		mainPanel = GameObject.Find ("MainMenuPanel");
		tipsPanel.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (!launchingGameReady)
			return;
		if (Input.GetKeyDown (KeyCode.Space)) {
			Application.LoadLevel ("BeginingLevel");
		}
	
	}

	public void launchTipsPanel(){
		launchingGameReady = true;
		mainPanel.SetActive (false);
		tipsPanel.SetActive (true);
	}

	public void quitButton(){
		Application.Quit ();
	}
}
