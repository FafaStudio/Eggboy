using UnityEngine;
using System.Collections;

public class Spike : Trap {

	private BoxCollider2D trigger;
	private Animator anim;
	private Player eggboy;

	public bool isPlayer = false;

	private int TurnCount = 2;

	protected override void Start () {
		base.Start ();
		trigger = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	public override void doAction ()
	{
		if (TurnCount != 0) {
			TurnCount--;
			return;
		}
		else{
			anim.SetTrigger ("isActioning");
			TurnCount = 2;
			isEnclenched = false;
			if (isPlayer) {
				eggboy.loseHP ();
				TurnCount = 1;
				isEnclenched = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			eggboy = col.gameObject.GetComponent<Player> ();
			isPlayer = true;
			isEnclenched = true;
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			isPlayer = false;
		}
	}


}
