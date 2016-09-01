using UnityEngine;
using System.Collections;

public abstract class Trap : MonoBehaviour {

	public bool isEnclenched = false;
	public bool isActioning = false;
	protected bool isPlayer = false;
	protected float TurnDelay = 0.1f;

	protected virtual void Start () {
		GameManager.instance.AddTrapToList (this);
	}

	void Update () {
	
	}

	//Coroutine pour déclencher les pièges pendant le tour des ennemis ( exemple : les spikes ) 
	public abstract void doAction ();

	public void setIsPlayer(bool value){
		isPlayer = value;
	}

    //Coroutine pour déclencher les pièges pendant le tour du joueur ( exemple : les flèches ) 
    //public abstract IEnumerator declencherPiege();
    public abstract void declencherPiege();
    public abstract void declencherPiegeNewTurn();


}
