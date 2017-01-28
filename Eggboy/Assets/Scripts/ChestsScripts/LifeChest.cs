using UnityEngine;
using System.Collections;

public class LifeChest : Chest {

	//NE DOIT LOOTER QUE DES GOLDS

	protected override void Start ()
	{
		base.Start ();
		chestName = "Life";
		GameManager.instance.setSecretRoomAccess (true);
		GameManager.instance.modifyCptLifeChest (true);
	}

	public override void openChest (){
		Loot chestLoot = lootChest ();
		player.gainGolds (chestLoot.getGolds (),1);
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
		int randomLoot = (int)Random.Range (0, 2);
		Loot loot = new Loot (0);
		switch (randomLoot) {
		case 0:
			loot.setGolds (15);
			break;
		case 1: 
			loot.setGolds (15);
			break;
		case 2:
			loot.setGolds (20);
			break;
		}
		return loot;
	}

	public override void declencherPiege (){}
	public override void declencherPiegeNewTurn (){}
	public override void doAction (){}
	public override void TriggerEnter (MovingObject trapped){}
	public override void TriggerExit (){}
	public override void boutonDeclenchement (){}
}
