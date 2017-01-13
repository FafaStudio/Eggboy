using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour {

	private bool isItemShop;

	public GameObject halfHeartItem;
	public GameObject fullHeartItem;
	public GameObject[] passifItems;

	private Vector3 slot1;
	private Vector3 slot2;
	private Vector3 slot3;
	private Vector3 slot4;

	//bool pour tester si il y a la carte gold dans le magasin, 
	//si oui, on attend le moment ou le joueur l'achète pour diviser le prix des items en vente par 2
	private bool isGoldCard = false;


	private List<GameObject> itemsOfShop;

	void Start () {
		passifItems = GameManager.instance.getGameInformations ().passifItems;
		
		slot1 = new Vector3 (4, 4, 0f);
		slot2 = new Vector3 (6, 4, 0f);
		slot3 = new Vector3 (8, 4, 0f);
		slot4 = new Vector3 (10, 4, 0f);

		itemsOfShop = new List<GameObject> ();

		if ((GameManager.instance.getInfosLevels().getLevelsCount()== 11)||
			(GameManager.instance.getInfosLevels().getLevelsCount()== 24)||
			(GameManager.instance.getInfosLevels().getLevelsCount()== 35)){
			isItemShop = true;
		} else {
			isItemShop = false;
		}
		//isItemShop = true;
		//GameObject.Find ("Player").GetComponent<Player> ().gainGolds (3000);*/
		initShop ();
	}
		
	public void initShop(){
		if (isItemShop) {
			int randomItem1 = Random.Range (0, 2);
			int randomItem2 = Random.Range (0, 3);
			int randomItem3 = Random.Range (0, 3);
			int randomItem4 = Random.Range (0, passifItems.Length);

			GameObject itemToSpawn = new GameObject ();

			if (randomItem1 < 2) {
				spawnNewObject (halfHeartItem, slot1);
			} else {
				spawnNewObject (fullHeartItem, slot1);
			}

			if (randomItem2 <= 2) {
				spawnNewObject (fullHeartItem, slot2);
			} else {
				randomItem2 = Random.Range (0, passifItems.Length);
				itemToSpawn = GameManager.instance.getGameInformations().choosePassifItem(passifItems [randomItem2]);
				spawnNewObject (itemToSpawn, slot2);
			}
			if (randomItem3 <= 2) {
				itemToSpawn = GameManager.instance.getGameInformations ().choosePassifItem (passifItems[(int)Random.Range(0f, passifItems.Length)]);
				spawnNewObject (itemToSpawn, slot3);
			} else {
				spawnNewObject (fullHeartItem, slot3);
			}
			itemToSpawn = GameManager.instance.getGameInformations ().choosePassifItem (passifItems[randomItem4]);
			spawnNewObject (itemToSpawn, slot4);
		} else {
			spawnNewObject (halfHeartItem, slot1+new Vector3(1,0));
			spawnNewObject (fullHeartItem, slot2+new Vector3(1,0));
			spawnNewObject (fullHeartItem, slot3+new Vector3(1,0));
		}
	}

	public void spawnNewObject(GameObject itemToSpawn, Vector3 position){
		GameObject toInstantiate = Instantiate(itemToSpawn, position, Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<Item> ().setShop (this);
		toInstantiate.GetComponent<Item> ().isShopItem = true;
		toInstantiate.transform.SetParent (this.transform);
		itemsOfShop.Add (toInstantiate);
		isGoldCard= testGoldCardItem ();
		if(GameManager.instance.PlayerHasItem ("EggGoldCard"))
		//si le joueur a l'item egggoldcard en arrivant dans le shop ou lorsqu'un nouvel item y apparait
			toInstantiate.GetComponent<Item> ().price /= 2;
	}

	public bool testGoldCardItem(){
	// teste si le joueur achète la carte gold, 
	// si retourne faux, c'est que le joueur l'a acheté et on divise tous les prix par 2
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
		StartCoroutine (waitForPlayerGoldCard());
	}

	public IEnumerator waitForPlayerGoldCard(){
		isGoldCard = false;
		while (testGoldCardItem ()) {
			yield return new WaitForSeconds (0.25f);
		}
		for (int i = 0; i < itemsOfShop.Count; i++) {
			if (itemsOfShop.Count!=3 && i == itemsOfShop.Count - 1)
				yield return null;
			else
				itemsOfShop [i].GetComponent<Item> ().price /= 2;
		}
	}


	public List<GameObject> getItemsOfShop(){
		return itemsOfShop;
	}
}
