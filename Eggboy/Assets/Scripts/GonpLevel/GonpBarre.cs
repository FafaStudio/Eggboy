using UnityEngine;
using System.Collections;

public class GonpBarre : Enemy {

	private Rigidbody2D body;

	void Start () {
		body = this.GetComponent<Rigidbody2D> ();
		if (this.gameObject.name == "barreLeft") {
			speedForce = -speedForce;
			gameObject.transform.position = new Vector2(-56f, 0f);
		}
		else
			gameObject.transform.position = new Vector2(56f, 0f);
	}

	void FixedUpdate () {
		doMovement ();
	}

	protected override void doMovement(){
		body.velocity = new Vector2 (0f, speedForce);
	}

	protected override void doAttack() {
		
	}

	void OnCollisionEnter2D(Collision2D col){
		//if (col.gameObject.tag == "Wall") {
			speedForce = -speedForce;
		//}
	}
}
