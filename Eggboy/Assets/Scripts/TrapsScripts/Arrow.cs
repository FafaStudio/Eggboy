﻿using UnityEngine;
using System.Collections;

public class Arrow : Trap {

    private Animator anim;
	private GameObject character;

    public enum Direction {Nord, Est, Sud, Ouest};
    public Direction dir;

    public Sprite[] sprites;

    private GameManager manager;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        manager = GameManager.instance;
        setSprite();
    }

    public override void doAction()
    {
        return;
    }

	public override void TriggerEnter(MovingObject col){
		if ((col.gameObject.GetComponent<MovingObject>() != null)&&(!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
			isEnclenched = true;
			character = col.gameObject;
			character.GetComponent<MovingObject>().setIsTrap(true);
			character.GetComponent<MovingObject>().piege = this;
        }
    }

	public override void TriggerExit(){
		isEnclenched = false;
		if (character != null) {
			character.GetComponent<MovingObject> ().setIsTrap (false);
			character.GetComponent<MovingObject> ().piege = null;
		}
	}
		
    public override void declencherPiege()
    {
        switch (dir)
        {
            case Direction.Nord: 
				character.GetComponent<MovingObject>().doMove(0, 1);
                break;
            case Direction.Est:
				character.GetComponent<MovingObject>().doMove(1, 0);
                break;
            case Direction.Sud:
				character.GetComponent<MovingObject>().doMove(0, -1);
                break;
            case Direction.Ouest:
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
            case Direction.Nord:
                this.GetComponent<SpriteRenderer>().sprite = sprites[0];
                break;
            case Direction.Est:
                this.GetComponent<SpriteRenderer>().sprite = sprites[1];
                break;
            case Direction.Sud:
                this.GetComponent<SpriteRenderer>().sprite = sprites[2];
                break;
            case Direction.Ouest:
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
            case Direction.Nord:
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, 1, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Est:
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(1, 0, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Sud:
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(0, -1, 0), new Vector3(0.93f, 0.93f, 1));
                break;
            case Direction.Ouest:
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(1, 1, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(0.97f, 0.97f, 1));
                Gizmos.DrawWireCube(transform.position + new Vector3(-1, 0, 0), new Vector3(0.93f, 0.93f, 1));
                break;
        }
    }
}
