using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPlayer : MonoBehaviour {

	private Player playerManager;
	public GameObject[] heartContainer;
	private List<GameObject> instantiateHeart;// 0 full, 1 demi, 2 vide

	private Text goldsContainer;

	private Text curLevel;

	private GameObject updateGoldPanel;
	private Text comboText;
	private Text updateGoldsText;

	void Awake () {
		playerManager = GameObject.Find ("Player").GetComponent<Player> ();
		instantiateHeart = new List<GameObject> ();
		goldsContainer = GameObject.Find ("Golds").GetComponent<Text> ();
		curLevel = GameObject.Find ("LevelText").GetComponent<Text> ();
		updateGoldPanel = GameObject.Find ("updateGolds").gameObject;
		updateGoldsText = updateGoldPanel.GetComponentsInChildren<Text> () [0];
		comboText = updateGoldPanel.GetComponentsInChildren<Text> () [1];
		updateGoldPanel.SetActive (false);
	}
		
	public void updateLife(){
		resetLifeContainers ();
		switch(playerManager.getHp()){
		case 1:
			instantiateLifeContainers (heartContainer [1]);
			instantiateLifeContainers (heartContainer [2]);
			instantiateLifeContainers (heartContainer [2]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 2:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [2]);
			instantiateLifeContainers (heartContainer [2]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 3:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			instantiateLifeContainers (heartContainer [2]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 4:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [2]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			}else if (GameManager.instance.maxPlayerHpPoints == 10) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 5:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			} else if (GameManager.instance.maxPlayerHpPoints == 10) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 6:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			if (GameManager.instance.maxPlayerHpPoints == 8) {
				instantiateLifeContainers (heartContainer [2]);
			}else if (GameManager.instance.maxPlayerHpPoints == 10) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 7:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			if (GameManager.instance.maxPlayerHpPoints == 10) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 8:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			if (GameManager.instance.maxPlayerHpPoints == 10) {
				instantiateLifeContainers (heartContainer [2]);
			}
			break;
		case 9:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			break;
		case 10:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			break;
		}

	}

	public void updateGolds(){
		goldsContainer.text = playerManager.getGolds ().ToString ();
	}

	private void instantiateLifeContainers(GameObject toInstantiate){
		GameObject instantiation = Instantiate (toInstantiate);
		instantiation.transform.SetParent (this.gameObject.transform);
		instantiateHeart.Add (instantiation);
	}

	private void resetLifeContainers(){
		for (int i = 0; i < instantiateHeart.Count; i++) {
			Destroy (instantiateHeart [i]);
		}
		instantiateHeart.Clear ();
	}

	public void updateLevel(int level){
		//curLevel.text = (47-level).ToString ();
		curLevel.text = level.ToString();
	}

	public IEnumerator updateGoldsLaunch(int golds, int combo, bool goldsAdd){
		if (goldsAdd) {
			if (combo < 2)
				comboText.text = "";
			else
				comboText.text = "x" + combo.ToString ();
			updateGoldsText.color = new Color (0,1,0,1);
			updateGoldsText.text = "+" + golds.ToString ();
			updateGoldPanel.SetActive (true);
			yield return new WaitForSeconds (1f);
			updateGolds ();
			updateGoldPanel.SetActive (false);
		} else {
			comboText.text = "";
			updateGoldsText.color = new Color (1,0,0,1);
			updateGoldsText.text = "-" + golds.ToString ();
			updateGoldPanel.SetActive (true);
			yield return new WaitForSeconds (1f);
			updateGolds ();
			updateGoldPanel.SetActive (false);
		}
		
	}
}
