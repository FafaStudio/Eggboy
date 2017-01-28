using UnityEngine;
using System.Collections;

public class SecretChest : Chest {

	protected override void Start ()
	{
		base.Start ();
		chestName = "Secret";
	}

	public override void openChest (){
		Loot chestLoot = lootChest ();
		//player.gainGolds (chestLoot.getGolds (),1);
		destroyChest ();
	}

	public override void destroyChest ()
	{
		GameManager.instance.modifyCptLifeChest (false);
		if(GameManager.instance.getCptLifeChestCurrent()==0)
			GameManager.instance.setSecretRoomAccess (false);
		base.destroyChest ();
	}

	public override Loot lootChest(){
		GameObject itemToSpawn = GameManager.instance.getGameInformations().choosePassifItem(GameManager.instance.getGameInformations ().passifItems[(int)Random.Range(0f, (float)GameManager.instance.getGameInformations ().passifItems.Length)]);
		GameObject toInstantiate = Instantiate(itemToSpawn, this.transform.position, Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<Item> ().isShopItem = false;
		toInstantiate.GetComponent<Item> ().setIsSecretItem (true);
		if (GameManager.instance.enemies.Count > 1) {
			GameManager.instance.RemoveEnemyToList (this.GetComponent<Blob> ());
		}
		return new Loot(0);
	}

	public override void declencherPiege (){}
	public override void declencherPiegeNewTurn (){}
	public override void doAction (){}
	public override void TriggerEnter (MovingObject trapped){}
	public override void TriggerExit (){}
	public override void boutonDeclenchement (){}
}
