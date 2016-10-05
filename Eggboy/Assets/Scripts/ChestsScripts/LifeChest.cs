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
		print ("Gagné");
		destroyChest ();
	}

	public override void declencherPiege (){}
	public override void declencherPiegeNewTurn (){}
	public override void doAction (){}
	public override void TriggerEnter (MovingObject trapped){}
	public override void TriggerExit (){}
	public override void boutonDeclenchement (){}
}
