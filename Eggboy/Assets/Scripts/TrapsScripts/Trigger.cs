using UnityEngine;
using System.Collections;
using System;


public class Trigger : Trap {

	public Sprite off;
	public Sprite On;
	
    public Trap[] cibles;

	public Sprite[] colors;// 0:vert, 1:bleu, 2:rouge, 3:violet
	public Sprite[] halfColors;// 0:vert, 1:bleu, 2:rouge, 3:violet

	public GameObject colorReceptacle;
	public enum color{vert, bleu, rouge, violet};
	public color chooseColor;

	protected override void Start ()
	{
		base.Start ();
		setTriggerColor ();
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
		GetComponent<SpriteRenderer> ().sprite = off;
		if (character != null) {
			character.setIsTrap (false);
			character.piege = null;
		}
	}

	public void setTriggerColor(){
		GameObject toInstantiate = new GameObject ();
		switch (chooseColor) {
		case color.vert:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [0];
			toInstantiate.transform.SetParent (this.transform);
			setTrapsColor(colors [0]);
			break;
		case color.bleu:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [1];
			toInstantiate.transform.SetParent (this.transform);
			setTrapsColor(colors [1]);
			break;
		case color.rouge:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [2];
			toInstantiate.transform.SetParent (this.transform);
			setTrapsColor(colors [2]);
			break;
		case color.violet:
			toInstantiate = Instantiate (colorReceptacle, this.transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = colors [3];
			toInstantiate.transform.SetParent (this.transform);
			setTrapsColor(colors [3]);
			break;
		}
	}

	public void setTrapsColor(Sprite color){
		for (int i = 0; i < cibles.Length; i++) {
			GameObject toInstantiate = Instantiate (colorReceptacle, cibles[i].transform.position, Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<SpriteRenderer> ().sprite = color;
			toInstantiate.transform.SetParent (this.transform);
			if (cibles [i].setColorsBoutons (toInstantiate) > 1) {
				switch (chooseColor) {
				case Trigger.color.vert:
					toInstantiate.GetComponent<SpriteRenderer> ().sprite = halfColors [0];
					toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
					break;
				case Trigger.color.bleu:
					toInstantiate.GetComponent<SpriteRenderer> ().sprite =halfColors [1];
					toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
					break;
				case Trigger.color.rouge:
					toInstantiate.GetComponent<SpriteRenderer> ().sprite =halfColors [2];
					toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
					break;
				case Trigger.color.violet:
					toInstantiate.GetComponent<SpriteRenderer> ().sprite =halfColors [3];
					toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder++;
					break;
				}
			}
		}
	}

    public override void declencherPiege(){
		GetComponent<SpriteRenderer> ().sprite = On;
		for (int i = 0; i < cibles.Length; i++) {
			cibles [i].boutonDeclenchement ();
		}
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
