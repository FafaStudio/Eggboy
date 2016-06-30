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

	public abstract void doAction ();

	public void setIsPlayer(bool value){
		isPlayer = value;
	}

    public abstract IEnumerator declencherPiege();



}
