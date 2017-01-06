using UnityEngine;
using System.Collections;
using System;

public class Spike : Trap {

	private Animator anim;
	public bool isButton;
	private bool boutonEnclenched = false;

	public int TurnCount = 2;

	public int offsetDepart;

	//protected Vector2 screenPos;

	UICompteur compteur;

	protected override void Start () {
		if (isButton) {
			Destroy (this.GetComponent<UICompteur> ());
		} 
		compteur = this.GetComponent<UICompteur> ();

		isEnclenched = true;
		base.Start ();
		//screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
		anim = GetComponent<Animator> ();
	}

	public override void doAction (){
		if (isButton)
			return;
		if (offsetDepart > 0) {
			offsetDepart--;
			return;
		}
		if (TurnCount > 0) {
			TurnCount--;
		} else {
			isActioning = true;
			anim.SetBool ("isActioning", true);
			isActioning = true;
			declencherPiege ();
		}
	}

	public override void TriggerEnter(MovingObject col){
		isEnclenched = true;
		isCharacter = true;
		character = col;
		character.piege = this;
		character.setIsTrap (true);
		if (character.gameObject.tag == "Player") 
			GameManager.instance.playersTurn = false;
	}

	public override void TriggerExit(){
		isCharacter = false;
		character.piege = null;
		character.setIsTrap (false);
		character = null;
	}

	public void resetSpike(){
		if (isButton)
			return;
		isActioning = false;
		anim.SetBool ("isActioning", false);
		TurnCount = 2;
	}

    public override void declencherPiege(){
		if (character == null)
			return;
		character.GetComponent<MovingObject> ().setIsUnderTrapEffect (false);
		if (isActioning) {
			if (character.gameObject.tag == "Player") {
				character.GetComponent<Player> ().loseHP ();
			} else if (character.gameObject.tag == "Enemy") {
				character.GetComponent<Enemy> ().Die ();
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
			isEnclenched = false;
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
			isEnclenched = true;
		}
	}

    void OnDrawGizmos()
    {     
        if (isEnclenched)//affiche le nombre de tours restant avant activation
        {
            Gizmos.DrawIcon(transform.position, "Numbers_"+(TurnCount+1)+".jpg", true);
        }
    }

	void Update(){
		if (isActioning || boutonEnclenched || isButton) {
			compteur.disactiveUI ();
			compteur.setInformation ("0");
		}
		else {
			compteur.activeUI ();
			compteur.setInformation((TurnCount+1+offsetDepart).ToString ());
		}
	}

}
