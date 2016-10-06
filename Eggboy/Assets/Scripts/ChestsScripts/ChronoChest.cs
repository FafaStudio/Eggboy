using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChronoChest : Chest {

	//LOOT DES GOLDS OU RAREMENT UN ITEM
	//OU
	//TRES RAREMENT LES DEUX

	public GameObject uiToInstantiate;
	private GameObject instantiateUI;

	public int chrono;

	protected override void Start (){
		base.Start ();
		//instantiateUI = Instantiate(uiToInstantiate, new Vector3((int)this.transform.position.x, (int)this.transform.position.y, 0), Quaternion.identity) as GameObject;
		//instantiateUI.GetComponent<Text>().text = chrono.ToString();
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
