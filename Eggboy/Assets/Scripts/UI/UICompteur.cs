using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UICompteur : MonoBehaviour {

	GameObject uiContainer;
	GameObject textContainer;

	private Text uiText;

	void Start(){
		uiContainer = Instantiate(Resources.Load ("Canvas"),this.transform,true) as GameObject;
		textContainer = uiContainer.GetComponentInChildren<Image> ().GetComponentInChildren<Text> ().gameObject;
		uiText = textContainer.GetComponent<Text> ();
		uiContainer.GetComponent<RectTransform> ().position = this.transform.position;
	}

	void Update () {
		if (GameManager.instance.isInfoUI) {
			activeUI ();
		} else {
			disactiveUI ();
		}
	}

	public void setInformation(string info){
		if (int.Parse (info) > 9)
			uiText.GetComponent<RectTransform> ().position = new Vector3 (transform.position.x+0.3f, transform.position.y, 0);
		else
			uiText.GetComponent<RectTransform> ().position = new Vector3 (transform.position.x, transform.position.y, 0);
		uiText.text = info;
	}

	public void activeUI(){
		uiContainer.SetActive (true);
	}

	public void disactiveUI(){
		uiContainer.SetActive (false);
	}
}
