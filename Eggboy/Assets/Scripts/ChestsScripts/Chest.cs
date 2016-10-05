using UnityEngine;
using System.Collections;

public abstract class Chest : Trap {

	protected string chestName;
	
	public abstract void openChest ();

	public void destroyChest(){
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}

	public string getChestName(){
		return chestName;
	}
}
