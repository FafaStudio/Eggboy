﻿using UnityEngine;
using System.Collections;

public abstract class Trap : MonoBehaviour {

	//character trap by the trap
	protected MovingObject character;

	public bool isEnclenched = false;
	public bool isActioning = false;
	protected bool isCharacter = false;
	protected float TurnDelay = 0.1f;

	protected virtual void Start () {
		GameManager.instance.getCurrentBoard ().setNodeOnGrid ((int)transform.position.x, (int)transform.position.y, 1, this);
		GameManager.instance.AddTrapToList (this);
	}

	//fonction de detection appelées par les objets qui testent les cases lors d'un déplacement
	public abstract void TriggerEnter (MovingObject trapped);
	//fonction de réinitialisation du piège lorsque le personnage piégé sort du piège
	public abstract void TriggerExit ();

	//Coroutine pour déclencher les pièges pendant le tour des ennemis ( exemple : les spikes ) 
	public abstract void doAction ();

	public void setIsCharacter(bool value){
		isCharacter = value;
	}

    //Coroutine pour déclencher les pièges pendant le tour du joueur ( exemple : les flèches ) 
    //public abstract IEnumerator declencherPiege();
    public abstract void declencherPiege();
    public abstract void declencherPiegeNewTurn();


}
