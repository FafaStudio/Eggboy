using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour {

	private bool isItemShop;
	public bool isPlaytestVersion;

	public GameObject halfHeartItem;
	public GameObject fullHeartItem;
	public GameObject[] passifItems;

	void Start () {
		if (GameManager.instance.levelPassedCount () % 10 == 0) {
			isItemShop = true;
		} else {
			isItemShop = false;
		}
		//isItemShop = true;
		initShop ();
	}
		
	public void initShop(){
		if (isPlaytestVersion) {
			GameObject toInstantiate = Instantiate (halfHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
			toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
			toInstantiate = Instantiate (fullHeartItem, new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
			toInstantiate.transform.SetParent (this.transform);
			toInstantiate.GetComponent<Item> ().isShopItem = true;
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
				} else {
					toInstantiate = Instantiate (fullHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
				}
				if (randomItem2 < 3) {
					toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
				} else {
					randomItem2 = Random.Range (0, passifItems.Length);
					toInstantiate = Instantiate (passifItems[randomItem2], new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
					toInstantiate.transform.SetParent (this.transform);
					toInstantiate.GetComponent<Item> ().isShopItem = true;
				}
				toInstantiate = Instantiate (passifItems[randomItem3], new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;

			} else {
				GameObject toInstantiate = Instantiate (halfHeartItem, new Vector3 (5, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate = Instantiate (fullHeartItem, new Vector3 (7, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
				toInstantiate = Instantiate (fullHeartItem, new Vector3 (9, 4, 0f), Quaternion.identity) as GameObject;
				toInstantiate.transform.SetParent (this.transform);
				toInstantiate.GetComponent<Item> ().isShopItem = true;
			}
		}
	}
}
