using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	protected float inverseMoveTime;

	protected virtual void Start () {
	// protected virtual permet "l'override" par les classes qui héritent, pour pouvoir y modifier le start
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}

	protected virtual bool Move(int xDir, int yDir, out RaycastHit2D hit){
	//simule/teste le mouvement du personnage
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	protected virtual void AttemptMove(int xDir, int yDir){
	//literallement "tente de bouger", teste si il y a un truc qui gène le déplacement et l'identifie
		RaycastHit2D hit;
		bool canMove = Move(xDir, yDir, out hit);
		if (hit.transform == null) {
			return;
		} else if (!canMove) {
				OnCantMove (hit.transform.gameObject);
			}
	}

	protected virtual IEnumerator SmoothMovement(Vector3 end){
	//coroutine permettant de bouger une unité d'un espace/une case 
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}

	}

    

	protected abstract void OnCantMove (GameObject col);
}
