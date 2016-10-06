using UnityEngine;
using System.Collections;

public class Zombi : Enemy {

	private Necromancer papa;

	protected override void Start ()
	{
		enemyName = "basic";
		skipMove = true;
		base.Start ();
	}

	protected override IEnumerator SmoothMovement(Vector3 end)
	{
		//coroutine permettant de bouger une unité d'un espace/une case 
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		testPiege ();
	}

	protected override bool Move (int xDir, int yDir, out RaycastHit2D hit){
		skipMove = true;
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2 (xDir, yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;
		if (hit.transform == null) {
			if (piege != null) {
				piege.TriggerExit ();
			}
			caseExacte = new BoardManager.Node(1, new Vector2(transform.position.x + xDir, transform.position.y + yDir));
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)end.x, (int)end.y, -1, this);
			GameManager.instance.getCurrentBoard ().setCharacterOnGrid((int)transform.position.x, (int)transform.position.y, 1,null);
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	public void setNecroPere(Necromancer pere){
		papa = pere;
	}

	public override void Die(){
		papa.spawned.Remove (this.gameObject);
		base.Die ();
	}

	protected override void AttemptMove (int xDir, int yDir)
	{
		if (piege != null) {
			if (piege.gameObject.name == "Arrow") {
				skipMove = false;
			}
		}
		base.AttemptMove(xDir, yDir);
	}
}
