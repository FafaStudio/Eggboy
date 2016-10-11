using UnityEngine;
using System.Collections;
using System;

public class Spike : Trap {

	private Animator anim;
	private bool boutonEnclenched = false;

	private int TurnCount = 2;

	protected override void Start () {
		base.Start ();
		//trigger = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();
	}

	public override void doAction (){
		if (boutonEnclenched)
			return;
		if (isActioning) {
			anim.SetBool ("isActioning", false);
			isActioning = false;
			if (isCharacter) {
				isEnclenched = true;
			}
		}else 
		if (TurnCount != 0) {
			TurnCount--;
		}
		else if(isEnclenched){
			anim.SetBool ("isActioning", true);
			isActioning = true;
			isEnclenched = false;
			if (isCharacter) {
				if (character == null)
					return;
				if (character.gameObject.tag == "Player") {
					character.GetComponent<Player> ().loseHP ();
				} else if (character.gameObject.tag == "Enemy") {
					character.GetComponent<Enemy> ().Die ();
				}
				TurnCount = 2;
				isEnclenched = true;
			}
		}
	}

	public override void TriggerEnter(MovingObject col){
		isCharacter = true;
		character = col;
		character.piege = this;
		character.setIsTrap (true);
	}

	public override void TriggerExit(){
		isCharacter = false;
		character.piege = null;
		character.setIsTrap (false);
		character = null;
	}

    public override void declencherPiege(){
		character.setIsUnderTrapEffect(false);
		if (character.gameObject.tag == "Enemy") {
			if (isActioning) {
				character.GetComponent<Enemy> ().Die ();
			} else if (!isEnclenched) {
				TurnCount = 1;
				isEnclenched = true;
			} 
		} else {
			GameManager.instance.playersTurn = false;
			if (boutonEnclenched) {
				character.GetComponent<Player> ().loseHP ();
			}
			else if (!isEnclenched) {
				TurnCount = 2;
				isEnclenched = true;
			}
		}
    }

    public override void declencherPiegeNewTurn()
    {
    }

	public override void boutonDeclenchement (){
		if (!boutonEnclenched) {
			boutonEnclenched = true;
			anim.SetBool ("isActioning", true);
			isActioning = true;
			isEnclenched = true;
			if (isCharacter) {
				if (character == null)
					return;
				if (character.gameObject.tag == "Player") {
					character.GetComponent<Player> ().loseHP ();
				} else if (character.gameObject.tag == "Enemy") {
					character.GetComponent<Enemy> ().Die ();
				}
			}
		} else {
			boutonEnclenched = false;
			anim.SetBool ("isActioning", false);
			isActioning = false;
			TurnCount = 2;
			if (isCharacter) {
				isEnclenched = true;
			} else {
				isEnclenched = false;
			}
		}
	}

    void OnDrawGizmos()
    {     
        if (isEnclenched)//affiche le nombre de tours restant avant activation
        {
            Gizmos.DrawIcon(transform.position, "Numbers_"+(TurnCount+1)+".jpg", true);
        }
    }
}
