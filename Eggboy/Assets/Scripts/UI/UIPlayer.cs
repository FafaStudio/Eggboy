using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPlayer : MonoBehaviour {

	public GameObject[] heartContainer;
	private Player playerManager;
	private List<GameObject> instantiateHeart;
	private Text goldsContainer;

	void Awake () {
		playerManager = GameObject.Find ("Player").GetComponent<Player> ();
		instantiateHeart = new List<GameObject> ();
		goldsContainer = GameObject.Find ("Golds").GetComponent<Text> ();
	}

	void Update () {
	
	}

	public void updateLife(){
		resetLifeContainers ();
		switch(playerManager.getHp()){
		case 1:
			instantiateLifeContainers (heartContainer [1]);
			instantiateLifeContainers (heartContainer [2]);
			instantiateLifeContainers (heartContainer [2]);
			break;
		case 2:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [2]);
			instantiateLifeContainers (heartContainer [2]);
			break;
		case 3:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			instantiateLifeContainers (heartContainer [2]);
			break;
		case 4:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [2]);
			break;
		case 5:
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [0]);
			instantiateLifeContainers (heartContainer [1]);
			break;
		case 6:
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
}
