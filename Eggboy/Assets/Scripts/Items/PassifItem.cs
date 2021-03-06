﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PassifItem : Item {
	public string itemName;
	public string description;
	public bool isHpUp;

	public GameObject uiGainObject;

	protected override void Start ()
	{
		board = GameManager.instance.getCurrentBoard ();
		board.setObjectOnGrid((int)this.transform.position.x, (int)this.transform.position.y, -1, this.gameObject);
		base.Start ();
	}

	public override void GainLoot (Player player)
	{
		if (isShopItem) {
			if (player.getGolds () >= price) {
				if (isHpUp)
					hpUp (player);
				player.loseGolds (this.price);
				board.setObjectOnGrid ((int)this.transform.position.x, (int)this.transform.position.y, 1, null);
				GameManager.instance.AddPassifItem (this);
				this.transform.SetParent (GameManager.instance.gameObject.transform);
				Destroy (this.GetComponent<BoxCollider2D> ());
				Destroy (this.GetComponent<SpriteRenderer> ());
				removeFromShop ();
				StartCoroutine(seeUI ());
				isShopItem = false;
				if (GameManager.instance.PlayerHasItem ("Supermarket")) {
					GameObject itemToSpawn = GameManager.instance.getGameInformations ()
						.choosePassifItem (GameManager.instance.getGameInformations ().passifItems[(int)Random.Range(0f, GameManager.instance.getGameInformations ().passifItems.Length)]);
					GameObject.FindGameObjectWithTag ("Shop").GetComponent<ShopManager> ().spawnNewObject (itemToSpawn, this.transform.position);
				}
			}
		} else {
			if (isHpUp)
				hpUp (player);
			StartCoroutine(seeUI ());
			board.setObjectOnGrid ((int)this.transform.position.x, (int)this.transform.position.y, 1, null);
			GameManager.instance.AddPassifItem (this);
			this.transform.SetParent (GameManager.instance.gameObject.transform);
			if (isSecretItem) {
				if (GameManager.instance.enemies.Count == 1) {
					//si il n'y a plus d'ennemis
					// il est laissé a 1 par le secretChest afin de temporiser l'accès au niveau suivant
					//je passe au niveau suivant que si j'ai tué tous les ennemis et que j'ai pris l'item
				//	GameManager.instance.enemies.Clear ();
					if (GameManager.instance.enemies [0] == null)
						GameManager.instance.enemies.Clear ();
				}
				GameManager.instance.checkIfWinLevel ();
			}
			Destroy (this.GetComponent<BoxCollider2D> ());
			Destroy (this.GetComponent<SpriteRenderer> ());
		}
	}

	public void hpUp(Player player){
		if (GameManager.instance.maxPlayerHpPoints == 10)
			return;
		if (player.getHp () == GameManager.instance.maxPlayerHpPoints) {
			GameManager.instance.maxPlayerHpPoints += 2;
			player.gainHps (2);
			player.getUIPlayer ().updateLife ();
		} else {
			GameManager.instance.maxPlayerHpPoints += 2;
			player.getUIPlayer ().updateLife ();
		}
	}

	public IEnumerator seeUI(){
		GameObject toInstantiate = Instantiate (uiGainObject, new Vector3 (0, 0, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponentInChildren<Image> ().GetComponentsInChildren<Text> () [0].text = itemName;
		toInstantiate.GetComponentInChildren<Image> ().GetComponentsInChildren<Text> () [1].text = description;
		yield return new WaitForSeconds (2f);
		Destroy (toInstantiate);
	}
}
