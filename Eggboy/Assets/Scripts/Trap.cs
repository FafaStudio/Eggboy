using UnityEngine;
using System.Collections;

public abstract class Trap : MonoBehaviour {

	public bool isEnclenched = false;

	protected virtual void Start () {
		GameManager.instance.AddTrapToList (this);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public abstract void doAction ();
}
