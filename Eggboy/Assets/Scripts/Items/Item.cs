using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour {

	public bool isShopItem;
	public int price;
	private int constBasePrice;
	Vector2 screenPos;
	protected BoardManager board;

	protected ShopManager shop;

	protected virtual void Start () {
		constBasePrice = price;
		screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
	}

	public abstract void GainLoot (Player player);

	protected void removeFromShop(){
		shop.getItemsOfShop ().Remove (this.gameObject);
		this.shop = null;
	}

	public void setShop(ShopManager shopAdd){
		this.shop = shopAdd;
	}

	void OnGUI(){
		if (!isShopItem)
			return;
		
		var centeredStyle = GUI.skin.GetStyle("Label");
		if (GameManager.instance.PlayerHasItem ("EggGoldCard"))
			GUI.color = Color.red;
		else
			GUI.color = Color.yellow;
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.TextField (new Rect (screenPos.x-15, (Screen.height - screenPos.y) +20, 40, 20), price.ToString ());
	}

	public int getBasePrice(){
		return constBasePrice;
	}
}
