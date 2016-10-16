using UnityEngine;
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
			}
		} else {
			if (isHpUp)
				hpUp (player);
			StartCoroutine(seeUI ());
			board.setObjectOnGrid ((int)this.transform.position.x, (int)this.transform.position.y, 1, null);
			GameManager.instance.AddPassifItem (this);
			this.transform.SetParent (GameManager.instance.gameObject.transform);
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
