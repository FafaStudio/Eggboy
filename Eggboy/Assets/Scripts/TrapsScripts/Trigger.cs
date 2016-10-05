using UnityEngine;
using System.Collections;
using System;


public class Trigger : Trap {

	public Sprite off;
	public Sprite On;
	
    public Trap[] cibles;

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
			if (character.tag == "Player") {
				GameManager.instance.playersTurn = false;
			}
        }
    }

	public override void TriggerExit(){
		isEnclenched = false;
		GetComponent<SpriteRenderer> ().sprite = off;
		if (character != null) {
			character.setIsTrap (false);
			character.piege = null;
		}
	}

    public override void declencherPiege(){
		for (int i = 0; i < cibles.Length; i++) {
			cibles [i].boutonDeclenchement ();
		}
		GetComponent<SpriteRenderer> ().sprite = On;
		character.GetComponent<MovingObject> ().setIsUnderTrapEffect (false);
    }

	public override void boutonDeclenchement (){}

    public override void declencherPiegeNewTurn(){
    }

    void OnDrawGizmos()
    {
        if (cibles == null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
            Gizmos.DrawWireCube(transform.position, new Vector3(0.97f, 0.97f, 1));
            Gizmos.DrawWireCube(transform.position, new Vector3(0.93f, 0.93f, 1));
        }

    }

    void OnDrawGizmosSelected()
    {
        if (cibles != null)
        {
			for (int i = 0; i < cibles.Length; i++) {
				Gizmos.color = Color.white;
				Gizmos.DrawLine (transform.position, cibles[i].transform.position);
			}
        }
    }
}
