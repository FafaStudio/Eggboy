using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Necromancer : Enemy {

	public GameObject zombieRat;
	protected int cptZombie = 0;
	public int maxZombie = 3;

	public List<GameObject> spawned;

	private bool hasSpawnedLastTurn = false;


	protected override void Start ()
	{
		enemyName = "necro";
		spawned = new List<GameObject> ();
		animator = null;
		base.Start ();
	}

	public override void MoveEnemy ()
	{
		if (isDead)
			return;

		int xDir = 0;
		int yDir = 0;

		BoardManager.Node currentPos = new BoardManager.Node (1, new Vector2 (this.transform.position.x, this.transform.position.y));
		currentPos.distanceVO = currentPos.volDoiseau (target.GetComponent<Player>().caseExacte.position);
	
		if (currentPos.distanceVO < 4) {
			//	Vector2 nextPosition = GameManager.instance.getCurrentBoard().doPathfinding(target.GetComponent<Player>().caseExacte, currentPos);

			//If the difference in positions is approximately zero (Epsilon) do the following:
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? -1 : 1;
			else
				xDir = target.position.x > transform.position.x ? -1 : 1;
			hasSpawnedLastTurn = false;
			AttemptMove (xDir, yDir);
		} else if (!hasSpawnedLastTurn) {
			if (spawned.Count < maxZombie) {
				Vector2 spawnPosition = GameManager.instance.getCurrentBoard ().doPathfinding (target.GetComponent<Player> ().caseExacte, currentPos);
				if ((spawnPosition.x == -1f) && (spawnPosition.y == -1f)) {
					List<BoardManager.Node> voisins = GameManager.instance.getCurrentBoard ().Voisins (new BoardManager.Node(1, new Vector2((int)this.transform.position.x, (int)this.transform.position.y)));
					for (int i = 0; i < voisins.Count; i++) {
						if (voisins [i].valeur == 1) {
							spawnZombie ((int)voisins[i].position.x, (int)voisins[i].position.y);
							return;
						}
					}
				}else
					spawnZombie ((int)spawnPosition.x, (int)spawnPosition.y);
			}
		} else
			hasSpawnedLastTurn = false;
	}

	public void spawnZombie(int xDir, int yDir){
		GameObject toInstantiate = Instantiate (zombieRat, new Vector3 (xDir, yDir, 0f), Quaternion.identity) as GameObject;
		toInstantiate.GetComponent<Zombi>().setNecroPere(this.GetComponent<Necromancer>());
		spawned.Add(toInstantiate);

		hasSpawnedLastTurn = true;
	}

	protected override void OnCantMove (GameObject col)
	{
		if (col.gameObject.tag == "Wall") {
			return;
		} else if (col.gameObject.tag == "Player") {
			return;
		}
	}
}
