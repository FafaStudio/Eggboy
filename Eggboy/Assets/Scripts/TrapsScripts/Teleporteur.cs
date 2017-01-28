using UnityEngine;
using System.Collections;
using System;


public class Teleporteur : Trap {

	public GameObject coTeleporteur;

	public GameObject colorReceptacle;
	public Trigger.color chooseColor;

	public Sprite[] colors;// 0:vert, 1:bleu, 2:rouge, 3:violet

	protected override void Start ()
	{
		if (coTeleporteur != null) {
			coTeleporteur.GetComponent<Teleporteur> ().coTeleporteur = this.gameObject;
			setColor ();
		}
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
			TriggerExit ();
		}
    }

	public override void boutonDeclenchement (){}

    public override void declencherPiegeNewTurn(){
    }

	public void setColor(){
		GameObject toInstantiate = new GameObject ();
		switch (chooseColor) {
		case Trigger.color.vert:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [0];
			toInstantiate.transform.SetParent (this.transform);
			setTeleporteurColor(colors [0]);
			break;
		case Trigger.color.bleu:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [1];
			toInstantiate.transform.SetParent (this.transform);
			setTeleporteurColor(colors [1]);
			break;
		case Trigger.color.rouge:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [2];
			toInstantiate.transform.SetParent (this.transform);
			setTeleporteurColor(colors [2]);
			break;
		case Trigger.color.violet:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [3];
			toInstantiate.transform.SetParent (this.transform);
			setTeleporteurColor(colors [3]);
			break;
		}
	}

	public void setTeleporteurColor(Sprite color){
		GameObject toInstantiate = Instantiate (colorReceptacle, coTeleporteur.transform.position, Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<SpriteRenderer> ().sprite = color;
		toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
		toInstantiate.transform.SetParent (coTeleporteur.transform);

		toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<SpriteRenderer> ().sprite = color;
		toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
		toInstantiate.transform.SetParent (this.transform);
	}

    void OnDrawGizmos()
    {

    }

    void OnDrawGizmosSelected()
    {
    }
}
