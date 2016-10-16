using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour {

	private bool isItemShop;
	public bool isPlaytestVersion;

	public GameObject halfHeartItem;
	public GameObject fullHeartItem;
	public GameObject[] passifItems;

	private bool isGoldCard = false;

	private List<GameObject> itemsOfShop;

	void Start () {
		itemsOfShop = new List<GameObject> ();
		if (GameManager.instance.levelPassedCount () % 10 == 0) {
			isItemShop = true;
		} else {
			isItemShop = false;
		}
	/*	isItemShop = true;
		GameObject.Find ("Player").GetComponent<Player> ().gainGolds (3000);*/
		initShop ();
	}
		
	public void initShop(){
		bool playerHasGoldCard = GameManager.instance.PlayerHasItem ("EggGoldCard");
		if (isPlaytestVersion) {
			GameObject toInstantiate = Instantiate (halfHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
			itemsOfShop.Add (toInstantiate);
			if (playerHasGoldCard)
				toInstantiate.GetComponent<Item> ().price /= 2;
			toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
			itemsOfShop.Add (toInstantiate);
			if (playerHasGoldCard)
				toInstantiate.GetComponent<Item> ().price /= 2;
			toInstantiate = Instantiate (fullHeartItem, new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
			itemsOfShop.Add (toInstantiate);
			if (playerHasGoldCard)
				toInstantiate.GetComponent<Item> ().price /= 2;
		} else {
			if (isItemShop) {
				int randomItem1 = Random.Range (0, 2);
				int randomItem2 = Random.Range (0, 3);
				int randomItem3 = Random.Range (0, passifItems.Length);
				GameObject toInstantiate = new GameObject ();
				if (randomItem1 < 2) {
					toInstantiate = Instantiate (halfHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
					toInstantiate.GetComponent<Item> ().setShop (this);
					itemsOfShop.Add (toInstantiate);
					if (playerHasGoldCard)
						toInstantiate.GetComponent<Item> ().price /= 2;
				} else {
					toInstantiate = Instantiate (fullHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
					toInstantiate.GetComponent<Item> ().setShop (this);
					itemsOfShop.Add (toInstantiate);
					if (playerHasGoldCard)
						toInstantiate.GetComponent<Item> ().price /= 2;
				}
				if (randomItem2 < 3) {
					toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
					toInstantiate.GetComponent<Item> ().setShop (this);
					itemsOfShop.Add (toInstantiate);
					if (playerHasGoldCard)
						toInstantiate.GetComponent<Item> ().price /= 2;
				} else {
					randomItem2 = Random.Range (0, passifItems.Length);
					toInstantiate = Instantiate (passifItems[randomItem2], new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
					toInstantiate.GetComponent<Item> ().setShop (this);
					itemsOfShop.Add (toInstantiate);
					if (playerHasGoldCard)
						toInstantiate.GetComponent<Item> ().price /= 2;
				}
				toInstantiate = Instantiate (passifItems[randomItem3], new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate.GetComponent<Item> ().setShop (this);
				itemsOfShop.Add (toInstantiate);
				if (playerHasGoldCard)
					toInstantiate.GetComponent<Item> ().price /= 2;

			} else {
				GameObject toInstantiate = Instantiate (halfHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate.GetComponent<Item> ().setShop (this);
				itemsOfShop.Add (toInstantiate);
				if (playerHasGoldCard)
					toInstantiate.GetComponent<Item> ().price /= 2;
				toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate.GetComponent<Item> ().setShop (this);
				itemsOfShop.Add (toInstantiate);
				if (playerHasGoldCard)
					toInstantiate.GetComponent<Item> ().price /= 2;
				toInstantiate = Instantiate (fullHeartItem, new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate.GetComponent<Item> ().setShop (this);
				itemsOfShop.Add (toInstantiate);
				if (playerHasGoldCard)
					toInstantiate.GetComponent<Item> ().price /= 2;
			}
		}
		isGoldCard= testGoldCardItem ();
	}

	public bool testGoldCardItem(){
		for (int i = 0; i < itemsOfShop.Count; i++) {
			if (itemsOfShop [i].GetComponent<PassifItem> () != null) {
				if (itemsOfShop [i].GetComponent<PassifItem> ().itemName == "EggGoldCard") {
					return true;
				}
			}
		}
		return false;
	}

	void Update(){
		if (!isGoldCard)
			return;
		if (!testGoldCardItem ()) {
			for (int i = 0; i < itemsOfShop.Count; i++) {
				itemsOfShop [i].GetComponent<Item> ().price /= 2;
			}
			isGoldCard = false;
		}
	}

	public List<GameObject> getItemsOfShop(){
		return itemsOfShop;
	}
}
