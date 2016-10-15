using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour {

	public bool isShopItem;
	public int price;
	Vector2 screenPos;

	protected virtual void Start () {
		screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
	}

	public abstract void GainLoot (Player player);

	void OnGUI(){
		if (!isShopItem)
			return;
		
		var centeredStyle = GUI.skin.GetStyle("Label");
		GUI.color = Color.yellow;
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.TextField (new Rect (screenPos.x-15, (Screen.height - screenPos.y) +20, 40, 20), price.ToString ());
	}
}
