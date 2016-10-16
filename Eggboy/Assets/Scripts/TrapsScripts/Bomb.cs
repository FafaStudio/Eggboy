using UnityEngine;
using System.Collections;

public class Bomb : Trap {

    private int turnCount = 1;
	private bool explosionIsLaunch;
	public GameObject explosion;

	protected override void Start(){
		GameManager.instance.AddTrapToList (this);
		isEnclenched = true;
	}

    public override void doAction()
    {
		if (turnCount > 0) {
			turnCount--;
		} else {
			launchExplosion ();
		}
    }
		

    public override void TriggerEnter(MovingObject col){
    }

	public override void TriggerExit(){
	}
		
    public override void declencherPiege()
    {
        isEnclenched = true;
    }

    public override void declencherPiegeNewTurn(){}
	public override void boutonDeclenchement (){}

	public void launchExplosion(){
		GameObject toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
		toInstantiate.transform.SetParent (this.transform);

		if (this.transform.position.x != 14) {
			toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x + 1, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
			toInstantiate.transform.SetParent (this.transform);
			if (this.transform.position.y != 7) {
				toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x+1, this.transform.position.y+1, 0f), Quaternion.identity) as GameObject;
				toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
				toInstantiate.transform.SetParent (this.transform);
			}
			if (this.transform.position.y != 0) {
				toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x+1, this.transform.position.y-1, 0f), Quaternion.identity) as GameObject;
				toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
				toInstantiate.transform.SetParent (this.transform);
			}
		}
		if (this.transform.position.x != 0) {
			toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x - 1, this.transform.position.y, 0f), Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
			toInstantiate.transform.SetParent (this.transform);

			if(this.transform.position.y!=7){
				toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x-1, this.transform.position.y+1, 0f), Quaternion.identity) as GameObject;
				toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
				toInstantiate.transform.SetParent (this.transform);
			}
			if (this.transform.position.y != 0) {
				toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x-1, this.transform.position.y-1, 0f), Quaternion.identity) as GameObject;
				toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
				toInstantiate.transform.SetParent (this.transform);
			}
		}
			
		if (this.transform.position.y != 7) {
			toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y + 1, 0f), Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
			toInstantiate.transform.SetParent (this.transform);
		}

		if (this.transform.position.y != 0) {
			toInstantiate = Instantiate (explosion, new Vector3 (this.transform.position.x, this.transform.position.y - 1, 0f), Quaternion.identity) as GameObject;
			toInstantiate.GetComponent<BombExplosion> ().checkExplosion ();
			toInstantiate.transform.SetParent (this.transform);
		}

		this.GetComponent<SpriteRenderer> ().sprite = null;
		explosionIsLaunch = true;
	}

	public void resetAfterExplosion(){
		GameManager.instance.RemoveTrapToList (this);
		Destroy (this.gameObject);
	}

	public bool isExplosing(){
		return explosionIsLaunch;
	}

	public void setCompteur(int value){
		turnCount = value;
	}

}
