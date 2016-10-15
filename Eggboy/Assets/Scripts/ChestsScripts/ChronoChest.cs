using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChronoChest : Chest {

	//LOOT DES GOLDS OU RAREMENT UN ITEM
	//OU
	//TRES RAREMENT LES DEUX

	public int chrono;

	Vector2 screenPos;

	protected override void Start (){
		base.Start ();
		screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		isEnclenched = true;
		chestName = "Chrono";
		if(GameManager.instance.PlayerHasItem("MagicianWatch"))
			chrono =(int)(chrono*1.5);
	}

	public override void openChest (){
		Loot chestLoot = lootChest ();
		player.gainGolds (chestLoot.getGolds ());
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

	void OnGUI()
	{
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.TextField (new Rect (screenPos.x - 6, (Screen.height - screenPos.y) - 10, 20, 20), chrono.ToString ());
	}
}
