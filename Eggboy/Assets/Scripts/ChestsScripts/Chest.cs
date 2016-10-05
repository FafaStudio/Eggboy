using UnityEngine;
using System.Collections;

public abstract class Chest : Trap {

	public string chestName;
	
	public abstract void openChest ();

	public void destroyChest(){
		Destroy (this.gameObject);
	}
}
