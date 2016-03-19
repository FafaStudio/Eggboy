using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public bool isAttacking;

	public float speedForce = 100f;
	Rigidbody2D body;

	private int movex = 0;
	private int movey = 0;
	
	public Animator animatorEggboy;

	public bool isDead;

	void Start () {
		body = GetComponent<Rigidbody2D> ();
		animatorEggboy = GetComponent<Animator> ();
		animatorEggboy.SetInteger ("Position", 1);
	}
	
	void Update () {
		if (!isDead) {
			if (!isAttacking) {
				doMovement ();
			}
		}
	}

	void FixedUpdate ()
	{
		body.velocity = new Vector2 (movex * speedForce, movey * speedForce);
	}


	private void doMovement(){
		if (Input.GetKey (KeyCode.Q)) {
			movex = -1;
			movey = 0;
			animatorEggboy.SetInteger ("Position", 2);
			animatorEggboy.SetBool ("isRunning", true);
			
			//c rigolo: 
			//transform.Rotate (Vector3.forward * -90);

		} 
		else if (Input.GetKey (KeyCode.D)) {
			movex = 1;
			movey = 0;
			animatorEggboy.SetInteger ("Position", 1);
			animatorEggboy.SetBool ("isRunning", true);
		} 	
			else if (Input.GetKey (KeyCode.Z)) {
				movex = 0;
				movey = 1;
		} 
				else if (Input.GetKey (KeyCode.S)) {
					movex = 0;
					movey = -1;
			
		} 
					else {
						movex = 0;
						movey = 0;
						animatorEggboy.SetBool ("isRunning", false);
		}
	}

}
