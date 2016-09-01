using UnityEngine;
using System.Collections;
using System;

public class Marecage : Trap {

	private Player eggboy;

	public override void doAction()
	{
		return;
	}

	void OnTriggerEnter2D(Collider2D col)// Appelé à chaque frame a partir du moment ou une collision est la
	{
        if ((col.gameObject.tag == "Player")&&(!isEnclenched)) // N'agit que si le joueur a finit son tour
		{
			isEnclenched = true;
			eggboy = col.gameObject.GetComponent<Player>();
/*            if (!eggboy.enabled)
            {
                eggboy.enabled = true;
            }*/ 
			eggboy.setIsTrap(true);
			eggboy.piege = this;
		}
	}


	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			isEnclenched = false;
			eggboy.setIsTrap(false);
			eggboy.piege = null;
		}
	}

	public override void declencherPiege()
	{
        eggboy.setIsUnderTrapEffect(false);
        eggboy.setIsUnderTrapNewTurnEffect(true);
        //yield return null;
	}

    public override void declencherPiegeNewTurn()
    {
        eggboy.setIsTrap(false);
        eggboy.piege = null;
        eggboy.passTurn();
        eggboy.setIsUnderTrapNewTurnEffect(false);
    }

}
