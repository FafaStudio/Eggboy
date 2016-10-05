using UnityEngine;
using System.Collections;
using System;

public class Marecage : Trap {

	public override void doAction()
	{
		return;
	}

	public override void TriggerEnter(MovingObject col){
        if (!isEnclenched) // N'agit que si le joueur a finit son tour
		{
			isEnclenched = true;
			character = col;
			character.setIsTrap(true);
			character.piege = this;
		}
	}

	public override void TriggerExit(){
		isEnclenched = false;
		character.setIsTrap(false);
		character.piege = null;
	}

	public override void declencherPiege()
	{
        character.setIsUnderTrapEffect(false);
        character.setIsUnderTrapNewTurnEffect(true);
		if (character.gameObject.tag == "Player") {
			GameManager.instance.playersTurn = false;
		}
	}

    public override void declencherPiegeNewTurn()
    {
        character.setIsTrap(false);
		if (character.gameObject.tag == "Player") {
			character.GetComponent<Player> ().passTurn ();
		} else {
			character.GetComponent<Enemy> ().skipMove = true;
		}
        character.setIsUnderTrapNewTurnEffect(false);
    }

}
