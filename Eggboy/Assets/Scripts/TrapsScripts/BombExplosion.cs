using UnityEngine;
using System.Collections;

public class BombExplosion : Trap {

	public LayerMask blockingLayer;

	protected override void Start(){
		base.Start ();
		isEnclenched = true;
	}

    public override void doAction()
    {
    }

    public override void TriggerEnter(MovingObject col){
		character = col;
		character.setIsTrap (false);
		if (character.gameObject.tag == "Enemy") {
			character.GetComponent<Enemy> ().Die ();
		} else {
			character.GetComponent<Player> ().loseHP ();
		}
    }

	public override void TriggerExit(){
	}

    public override void declencherPiege(){
    }

    public override void declencherPiegeNewTurn(){
    }

	public void checkExplosion(){
		MovingObject perso = GameManager.instance.getCurrentBoard ().testCaseCharacterPiege ((int)this.transform.position.x, (int)this.transform.position.y);
		if (perso == null) {
			return;
		}
		if (perso.gameObject.tag == "Enemy") {
			perso.GetComponent<Enemy> ().Die ();
		}else if(perso.gameObject.tag == "Player") {
			perso.GetComponent<Player> ().loseHP ();
		}
	}

	public void resetAfterExplosion(){
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1, null);
		GameManager.instance.RemoveTrapToList (this);
	}
}
