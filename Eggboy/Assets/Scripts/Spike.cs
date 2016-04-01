using UnityEngine;
using System.Collections;

public class Spike : Trap {

	private BoxCollider2D trigger;
	private Animator anim;
	private Player eggboy;

	private int TurnCount = 1;

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
			TurnCount = 1;
			isEnclenched = false;
			if ((eggboy.transform.position.x == this.gameObject.transform.position.x) && (eggboy.transform.position.y == this.gameObject.transform.position.y)) {
				eggboy.loseHP ();
			}
		}
	}

	void Update () {
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			eggboy = col.gameObject.GetComponent<Player> ();
			isEnclenched = true;
		}
	}


}
