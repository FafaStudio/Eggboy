using UnityEngine;
using System.Collections;

public class LifeChest : Chest {

	//NE DOIT LOOTER QUE DES GOLDS

	protected override void Start ()
	{
		base.Start ();
		chestName = "Life";
	}

	public override void openChest (){
		Loot chestLoot = lootChest ();
		player.gainGolds (chestLoot.getGolds ());
		destroyChest ();
	}

	public override Loot lootChest(){
		int randomLoot = (int)Random.Range (0, 2);
		Loot loot = new Loot (0);
		switch (randomLoot) {
		case 0:
			loot.setGolds (2);
			break;
		case 1: 
			loot.setGolds (2);
			break;
		case 2:
			loot.setGolds (3);
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
