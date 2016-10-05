using UnityEngine;
using System.Collections;

public class ChronoChest : Chest {

	//LOOT DES GOLDS OU RAREMENT UN ITEM
	//OU
	//TRES RAREMENT LES DEUX

	public int chrono;

	protected override void Start (){
		base.Start ();
		isEnclenched = true;
		chestName = "Chrono";
	}

	public override void openChest (){
		print ("Gagné");
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

	public override void TriggerEnter (MovingObject trapped){}
	public override void TriggerExit (){}
	public override void boutonDeclenchement (){}
}
