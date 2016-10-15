using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

	protected GameManager manager;

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	protected GameObject blockingObject = null;

	protected BoxCollider2D boxCollider;
	protected Rigidbody2D rb2D;
	protected float inverseMoveTime;
	public BoardManager.Node caseExacte;

	[HideInInspector]
	public Trap piege;
	protected bool isTrap = false;
	protected bool underTrapEffect = false;//effet de piege immédiat
	protected bool underTrapNewTurnEffect = false;//effet de piege lors du nouveau tour

	protected virtual void Start () {
	// protected virtual permet "l'override" par les classes qui héritent, pour pouvoir y modifier le start
		boxCollider = GetComponent<BoxCollider2D> ();
		manager = GameManager.instance;
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1f / moveTime;
	}

	protected virtual bool Move(int xDir, int yDir){
	//simule/teste le mouvement du personnage
		Vector2 end = caseExacte.position + new Vector2 (xDir, yDir);
		if(manager.getCurrentBoard().gridPositions[(int)(caseExacte.position.x+xDir),(int)(caseExacte.position.y+yDir)].nodeObject!=null){
			StartCoroutine(SmoothMovement(end));
			return true;
		}
		return false;
	}

	protected virtual void AttemptMove(int xDir, int yDir){
	//literallement "tente de bouger", teste si il y a un truc qui gène le déplacement et l'identifie
		bool canMove = Move(xDir, yDir);
		if (blockingObject== null) {
			return;
		} else if (!canMove) {
			OnCantMove ();
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

	public virtual void doMove(int xDir, int yDir)
	{
		AttemptMove(xDir, yDir);
	}

	protected abstract void OnCantMove ();

	protected abstract void testPiege ();

	public void piegeAccess(){
		testPiege ();
	}


	public bool getisTrap()
	{
		return this.isTrap;
	}

	public void setIsTrap(bool b)
	{
		this.isTrap = b;
	}

	public bool getisUnderTrapEffect()
	{
		return this.underTrapEffect;
	}

	public void setIsUnderTrapEffect(bool b)
	{
		this.underTrapEffect = b;
	}

	public bool getisUnderTrapNewTurnEffect()
	{
		return this.underTrapNewTurnEffect;
	}

	public void setIsUnderTrapNewTurnEffect(bool b)
	{
		this.underTrapNewTurnEffect = b;
	}
}
