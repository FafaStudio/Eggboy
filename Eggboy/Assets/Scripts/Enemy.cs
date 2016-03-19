using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	protected float speedForce = 100f;

	void Start () {
	
	}

	void Update () {
		
	}

	void onCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == "Punch") {
			Destroy (this.gameObject);
		}
	}

	protected abstract void doMovement();
}
