using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICompteur : MonoBehaviour {

	GameObject uiContainer;
	GameObject textContainer;

	private Text uiText;

	void Start(){
		uiContainer = Instantiate(Resources.Load ("UICompteur"),this.transform,true) as GameObject;
		textContainer = uiContainer.GetComponentInChildren<Image> ().GetComponentInChildren<Image> ().GetComponentInChildren<Text> ().gameObject;
		uiText = textContainer.GetComponent<Text> ();
		textContainer = textContainer.GetComponentInParent<Image> ().GetComponent<RectTransform> ().gameObject;
	}

	void Update () {
		textContainer.GetComponent<RectTransform> ().position = new Vector2 ((this.transform.position.x+1)*87.5f, (this.transform.position.y+1)*85);
		if (GameManager.instance.isInfoUI) {
			activeUI ();
		} else {
			disactiveUI ();
		}
	}

	public void setInformation(string info){
		uiText.text = info;
	}

	public void activeUI(){
		uiContainer.SetActive (true);
	}

	public void disactiveUI(){
		uiContainer.SetActive (false);
	}
}
