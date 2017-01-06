using UnityEngine;
using System.Collections;
using System;


public class Teleporteur : Trap {

	protected override void Start ()
	{
		base.Start ();
	}

    public override void doAction(){
        return;
    }

    public override void TriggerEnter(MovingObject col){
        if (!isEnclenched) // N'agit que si le joueur a finit son tour
        {
			isEnclenched = true;
			character = col;
			character.setIsTrap(true);
			character.piege = this;
			if (character.tag == "Player") {
				GameManager.instance.playersTurn = false;
			}
        }
    }

	public override void TriggerExit(){
		isEnclenched = false;
		if (character != null) {
			character.setIsTrap (false);
			character.piege = null;
		}

	}
		
    public override void declencherPiege(){
		character.GetComponent<MovingObject> ().setIsUnderTrapEffect (false);
    }

	public override void boutonDeclenchement (){}

    public override void declencherPiegeNewTurn(){
    }

    void OnDrawGizmos()
    {

    }

    void OnDrawGizmosSelected()
    {
    }
}
