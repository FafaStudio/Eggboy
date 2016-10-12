using UnityEngine;
using System.Collections;
using System;

public class Camuflaje : Trap {

    public GameObject piegeCamoufle;

    public enum Direction { Nord, Est, Sud, Ouest };
    public Direction directionArrowEventuelle; // si le piège camouflay est une arrow, on doit set sa direction dans le level design

    protected override void Start () {
        base.Start();
    }

    public override void TriggerEnter(MovingObject col){
        if ((col.gameObject.GetComponent<MovingObject>() != null) && (!isEnclenched)) // N'agit que si le joueur a finit son tour
        {
            isEnclenched = true;
            character = col;
            character.setIsTrap(true);
            character.piege = this;
        }
    }

    public override void declencherPiege(){
        GameObject toInstantiate = Instantiate(piegeCamoufle, new Vector3((int)this.transform.position.x, (int)this.transform.position.y, 0), Quaternion.identity) as GameObject;
		if (toInstantiate.gameObject.name != "Arrow(Clone)") {
			//pour empêcher le joueur de pouvoir entrer un input quand il fait une série de flèche camouflé
			TriggerExit ();
			if (toInstantiate.gameObject.name == "Bombe(Clone)") {
				if(character.gameObject.tag=="Player")
					toInstantiate.GetComponent<Bomb> ().setCompteur (2);
				else
					toInstantiate.GetComponent<Bomb> ().setCompteur (1);
			}
		}else{
			toInstantiate.GetComponent<Arrow> ().dir = (Arrow.Direction)directionArrowEventuelle;
		} 
        toInstantiate.GetComponent<Trap>().TriggerEnter(character);
        toInstantiate.GetComponent<Trap>().declencherPiege();
        GameManager.instance.RemoveTrapToList(this);
        Destroy(this.gameObject);
    }

    public override void doAction(){
        return;
    }

    public override void TriggerExit()
    {
        isEnclenched = false;
        if (character != null){
			GameManager.instance.playersTurn = false;
            character.setIsTrap(false);
            character.setIsUnderTrapEffect(false);
            character.piege = null;
        }
    }

    public override void declencherPiegeNewTurn(){}

	public override void boutonDeclenchement (){}
}
