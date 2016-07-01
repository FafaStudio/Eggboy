using UnityEngine;
using System.Collections;
using System;

public class Spike : Trap {

	//private BoxCollider2D trigger;
	private Animator anim;
	private Player eggboy;

	private int TurnCount = 2;

	protected override void Start () {
		base.Start ();
		//trigger = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	public override void doAction ()
	{
		if (isActioning) {
			anim.SetBool ("isActioning", false);
			isActioning = false;
			TurnCount = 2;
			if (isPlayer) {
				isEnclenched = true;
			}
		}
		if (TurnCount != 0) {
			TurnCount--;
		}
		else if(isEnclenched){
			anim.SetBool ("isActioning", true);
			isActioning = true;
			isEnclenched = false;
			if (isPlayer) {
				print ("AI");
				eggboy.loseHP ();
				TurnCount = 1;
				isEnclenched = true;
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			isPlayer = true;
			if (isEnclenched)
				return;
			TurnCount = 2;
			eggboy = col.gameObject.GetComponent<Player> ();
			isEnclenched = true;
		}
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.gameObject.tag == "Player") {
			isPlayer = false;
		}
	}

    public override IEnumerator declencherPiege()
    {
        yield return null;
    }


}
