using UnityEngine;
using System.Collections;
using System;


public class Teleporteur : Trap {

	public GameObject coTeleporteur;

	protected override void Start ()
	{
		if (coTeleporteur != null)
			coTeleporteur.GetComponent<Teleporteur> ().coTeleporteur = this.gameObject;
		base.Start ();
	}

    public override void doAction(){
        return;
    }

    public override void TriggerEnter(MovingObject col){
       /* if (!isEnclenched) // N'agit que si le joueur a finit son tour
        {*/
			//isEnclenched = true;
			character = col;
			character.setIsTrap(true);
			character.piege = this;
			if (character.tag == "Player") {
				GameManager.instance.playersTurn = false;
			}
     //   }
    }

	public override void TriggerExit(){
		if (character != null) {
			character.setIsTrap (false);
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
			character.GetComponent<MovingObject> ().setIsUnderTrapEffect (false);
			character.piege = null;
			character = null;
		}

	}
		
    public override void declencherPiege(){
		if (coTeleporteur.GetComponent<Teleporteur> ().character == null) {
			character.transform.position = coTeleporteur.transform.position;
			GameManager.instance.getCurrentBoard ().setObjectOnGrid((int)coTeleporteur.transform.position.x, (int)coTeleporteur.transform.position.y, 1, character.gameObject);
			character.caseExacte.position = coTeleporteur.transform.position;
		}
		TriggerExit ();
    }

	public override void boutonDeclenchement (){}

    public override void declencherPiegeNewTurn(){
    }

	void Update(){
		this.GetComponent<SpriteRenderer> ().color = new Vector4 (UnityEngine.Random.Range(0,1),UnityEngine.Random.Range(0,1),UnityEngine.Random.Range(0,1),1);
	}
    void OnDrawGizmos()
    {

    }

    void OnDrawGizmosSelected()
    {
    }
}
