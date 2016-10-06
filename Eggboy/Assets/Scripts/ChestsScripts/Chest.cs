using UnityEngine;
using System.Collections;

public abstract class Chest : Trap {

	protected string chestName;
	
	public abstract void openChest ();

    protected override void Start()
    {
        GameManager.instance.getCurrentBoard().setNodeOnGrid((int)transform.position.x, (int)transform.position.y, -1, this);
		GameManager.instance.AddTrapToList (this);
    }

	public void destroyChest(){
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}

	public string getChestName(){
		return chestName;
	}
}
