using UnityEngine;
using System.Collections;
using System;

public class Arrow : Trap {

    private Animator anim;

    public enum Direction {Haut, Droite, Bas, Gauche};
    public Direction dir;

    public Sprite[] sprites;

    protected override void Start () {
        base.Start();
        setSprite();
    }

    public override void boutonDeclenchement(){
		switch (dir){
			case Direction.Haut: 
				dir = Direction.Droite;
				break;
			case Direction.Droite:
				dir = Direction.Bas;
				break;
			case Direction.Bas:
				dir = Direction.Gauche;
				break;
			case Direction.Gauche:
				dir = Direction.Haut;
				break;
		}
		setSprite ();
    }

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
		if (character != null) {
			character.setIsTrap (false);
			character.piege = null;
		}
	}
		
    public override void declencherPiege(){
		if (character.gameObject.tag == "Player") {
			GameManager.instance.playersTurn = true;
		} 
        switch (dir)
        {
            case Direction.Haut: 
				character.GetComponent<MovingObject>().doMove(0, 1);
                break;
            case Direction.Droite:
				character.GetComponent<MovingObject>().doMove(1, 0);
                break;
            case Direction.Bas:
				character.GetComponent<MovingObject>().doMove(0, -1);
                break;
            case Direction.Gauche:
				character.GetComponent<MovingObject>().doMove(-1, 0);
                break;
        }
        //eggboy.setIsUnderTrapEffect(false);
        //yield return null;
    }

    public void setSprite()
    {
        switch (dir)
        {
            case Direction.Haut:
                this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case Direction.Droite:
                this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case Direction.Bas:
                this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case Direction.Gauche:
                this.GetComponent<SpriteRenderer>().sprite = sprites[3];
                break;
        }
    }
    public override void declencherPiegeNewTurn()
    {
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        switch (dir)
        {
            case Direction.Haut:
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Droite:
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Bas:
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Gauche:
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(0.93f, 0.93f, 1));
                break;
        }
    }
}
