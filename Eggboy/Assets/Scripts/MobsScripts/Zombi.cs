using UnityEngine;
using System.Collections;

public class Zombi : Enemy {

	private Necromancer papa;

	protected override void Start ()
	{
		skipMove = true;
		base.Start ();
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit)
	{
		skipMove = true;
		return base.Move (xDir, yDir, out hit);
	}

	public void setNecroPere(Necromancer pere){
		papa = pere;
	}

	public override void Die(){
		papa.spawned.Remove (this.gameObject);
		base.Die ();
	}
}
