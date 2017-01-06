using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChronoChest : Chest {

	//LOOT DES GOLDS OU RAREMENT UN ITEM
	//OU
	//TRES RAREMENT LES DEUX

	public int chrono;

	UICompteur compteur;

	Vector2 screenPos;

	protected override void Start (){
		base.Start ();
		compteur = this.GetComponent<UICompteur> ();
		isEnclenched = true;
		chestName = "Chrono";
		if(GameManager.instance.PlayerHasItem("MagicianWatch"))
			chrono =(int)(chrono*1.5);
	}

	public override void openChest (){
		Loot chestLoot = lootChest ();
		player.gainGolds (chestLoot.getGolds (),1);
		destroyChest ();
	}

	public override void declencherPiege (){}
	public override void declencherPiegeNewTurn (){}

	public override void doAction (){
		if (chrono > 1) {
			chrono--;
		} else {
			destroyChest ();
		}
	}

	public override Loot lootChest(){
		int randomLoot = (int)Random.Range (0, 2);
		Loot loot = new Loot (0);
		switch (randomLoot) {
		case 0:
			loot.setGolds (20);
			break;
		case 1: 
			loot.setGolds (20);
			break;
		case 2:
			loot.setGolds (30);
			break;
		}
		return loot;
	}

	public override void TriggerEnter (MovingObject trapped){}
	public override void TriggerExit (){}
	public override void boutonDeclenchement (){}

	void Update(){
		if (!GameManager.instance.isInfoUI) {
			compteur.disactiveUI ();
			compteur.setInformation ("0");
		}
		else {
			compteur.activeUI ();
			compteur.setInformation((chrono.ToString ()));
		}
	}
}
