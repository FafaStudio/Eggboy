using UnityEngine;
using System.Collections;

public class GonpBall : Enemy {

	private Rigidbody2D body;

	private float directionX = 0f;
	private float directionY = 1f;

	void Start () {
		body = this.GetComponent<Rigidbody2D> ();
		doMovement ();
	}

	protected override void doMovement(){
		body.velocity = Vector2.right * speedForce;
		print (body.velocity.ToString ());
	}

	protected override void doAttack() {
		
	}

	float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight) {
		// ascii art:
		// ||  1 <- at the top of the racket
		// ||
		// ||  0 <- at the middle of the racket
		// ||
		// || -1 <- at the bottom of the racket
		return (ballPos.y - racketPos.y) / racketHeight;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (col.gameObject.name == "barreLeft") {
			directionY = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
			directionX = 1f;
			// Calculate direction, make length=1 via .normalized
			Vector2 dir = new Vector2(directionX, directionY).normalized;

			body.velocity = dir * speedForce;
		}
		if (col.gameObject.name == "barreRight") {
			// Calculate hit Factor
			directionY = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);			              
			directionX = -1f;
			// Calculate direction, make length=1 via .normalized
			Vector2 dir = new Vector2(directionX, directionY).normalized;
			
			// Set Velocity with dir * speed
			body.velocity = dir * speedForce;
		}
		if (col.gameObject.tag == "Wall") {
			if(col.gameObject.name =="Up"||col.gameObject.name =="Down"){
				directionY = -directionY;
				Vector2 dir = new Vector2(directionX, directionY).normalized;
				body.velocity = dir * speedForce;
			}
			else{
				directionX = -directionX;
				Vector2 dir = new Vector2(directionX, directionY).normalized;
				body.velocity = dir * speedForce;
			}
		}
	}
}
