using UnityEngine;
using System.Collections;

public abstract class Chest : Trap {

	public class Loot : MonoBehaviour {
		private int golds;
		// système de loot d'item ici

		public Loot( int goldsToLoot){
			this.golds = goldsToLoot;
		}

		public int getGolds(){
			return this.golds;
		}

		public void setGolds(int goldsToLoot){
			this.golds = goldsToLoot;
		}
	}

	protected string chestName;
	protected Player player;
	
	public abstract void openChest ();

    protected override void Start()
    {
		GameManager.instance.getCurrentBoard().setObjectOnGrid((int)transform.position.x, (int)transform.position.y, -1, this.gameObject);
		GameManager.instance.AddTrapToList (this);
    }

	public virtual void destroyChest(){
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}

	public string getChestName(){
		return chestName;
	}

	public abstract Loot lootChest ();

	public void setPlayer(Player playa){
		this.player = playa;
	}
}
