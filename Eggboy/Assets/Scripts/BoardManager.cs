using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class toInstantiate
	{
		public GameObject objectInstantiate;
		public int x;
		public int y;
	}

	public class Node
	{
		public int valeur = 0;
		public Vector2 position;
		public Node parent;
		public int distanceParcourue;
		public int distanceVO;
		public Trap casePiege = null;

		public Node(int x, Vector2 p){
			this.valeur = x;
			this.position = p;
			this.parent = null;
			distanceParcourue = 0;
			distanceVO = 0;
		}

		public Node(){}

		public void setValeur(int value){
			this.valeur = value;
		}

		public void setPiege(Trap value){
			this.casePiege = value;
		}

		public String toString(){
			return " [" + this.valeur.ToString () + "] ";
		}

		public int volDoiseau(Vector2 destination)
		{
			return ((int)(Mathf.Abs (Mathf.Sqrt((Mathf.Pow((destination.y - this.position.y),2f)) + (Mathf.Pow((destination.x - this.position.x),2f))))));
		}

		public int calculDepartCourant()
		{
			if (this.parent == null) {
				return 0;
			} else {
				return this.valeur + this.parent.calculDepartCourant();
			}
		}
			
	}

	public int columns = 15;
	public int rows = 8;

	public GameObject[] floorTiles;
	//public toInstantiate[] objectTiles;

	private Transform boardHolder;
	//boardHolder permet lorsqu'on fait spawn plein de trucs de garder la hierarchie clean
	private Transform objectInLevel;

	private Node[,] gridPositions;

	//VISUEL____
	public GameObject[] listeTester;
	public Transform testGameObject;
	//__________


	void InitialiseGrille(){
		gridPositions = new Node[columns, rows];
		for(int y = rows-1; y >= 0; y--){
			for (int x =0; x < columns; x++) {
				gridPositions[x,y] = new Node(1,new Vector2(x,y));
			}
		}
	}

	public String grilleToString(){
		String toString = " ";
		for(int y = rows-1; y >= 0; y--){
			for (int x =0; x < columns; x++) {
				if (x == columns - 1)
					toString += gridPositions [x, y].toString () + " \n";
				else
					toString += gridPositions [x, y].toString ();
			}
		}
		return toString;
	}

	public void setNodeOnGrid(int x, int y , int value){
		gridPositions [x, y].setValeur(value);
		gridPositions [x, y].setPiege (null);
	}

	public void setNodeOnGrid(int x, int y , int value, Trap piege){
		gridPositions [x, y].setValeur(value);
		gridPositions [x, y].setPiege (piege);
	}

	public void testCasePiege(MovingObject persoToTest){
		if (gridPositions [(int)persoToTest.caseExacte.position.x, (int)persoToTest.caseExacte.position.y].casePiege != null) {
			gridPositions [(int)persoToTest.caseExacte.position.x, (int)persoToTest.caseExacte.position.y].casePiege.TriggerEnter (persoToTest);
		} else {
			return;
		}
		
	}

	void boardSetup(){
		boardHolder = new GameObject ("Board").transform;
		for (int x = 0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
				/*GameObject toInstantiate;
				if (y % 2 != 0) {
					if ((x == 0) || (x % 2 == 0)) {
						toInstantiate = floorTiles [0];
					} else
						toInstantiate = floorTiles [1];
				} else {
					if ((x == 0) || (x % 2 == 0)) {
						toInstantiate = floorTiles [1];
					} else
						toInstantiate = floorTiles [0];
				}*/
				GameObject instantiate = Instantiate(toInstantiate, new Vector3(x,y,0f),Quaternion.identity ) as GameObject;
				instantiate.transform.SetParent(boardHolder);
			}
		}
	}

	/*void InitialiseLevelDesign(){
		objectInLevel = new GameObject ("ObjectInLevel").transform;
		for(int i = 0; i < objectTiles.Length; i++){
			Vector3 position = new Vector3 (objectTiles [i].x, objectTiles [i].y, 0f);
			GameObject instantiate = Instantiate (objectTiles [i].objectInstantiate, position, Quaternion.identity) as GameObject;
			instantiate.transform.SetParent (objectInLevel);
		}
	}*/

	public void SetupScene(int level){
		boardSetup ();
		InitialiseGrille ();
		//InitialiseLevelDesign ();
	}

	public Vector2 doPathfinding (Node destination, Node depart){
		Node path = findPath (destination, depart, new List<Node>(), new List<Node>());

		//VISUEL___
		/*
		Destroy (GameObject.Find("testGameObject"+(GameManager.instance.totalTurns-1).ToString()));
		testGameObject = new GameObject ("testGameObject"+GameManager.instance.totalTurns.ToString()).transform;
		GameObject toTest = Instantiate (listeTester [2], path.position, Quaternion.identity) as GameObject;
		toTest.transform.SetParent (testGameObject);
		*/
		//________

		if (path.parent != null) {
			while (path.parent.parent != null) {
				path = path.parent;

				//VISUEL____
				/*
				toTest = Instantiate (listeTester [2], path.position, Quaternion.identity) as GameObject;
				toTest.transform.SetParent (testGameObject);
				*/
				//___________
			}	
		}

		//VISUEL____
		/*
		toTest = Instantiate (listeTester [2], path.parent.position, Quaternion.identity) as GameObject;
		toTest.transform.SetParent (testGameObject);
		*/
		//_________

		resetDistanceGrille ();
		return path.position;
	}

	public Node findPath(Node destination, Node depart, List<Node> openList, List<Node> closedList){
		closedList.Add (depart);

		//VISUEL___
		//GameObject toTest = Instantiate (listeTester [1], depart.position, Quaternion.identity) as GameObject;
		//toTest.transform.SetParent (testGameObject);
		//__________

		if ((depart.position.x == destination.position.x) && (depart.position.y == destination.position.y)) {
			return depart;
		}
		List<Node> voisins = Voisins (depart);
		for (int j = 0; j < voisins.Count; j++) {
			if ((voisins [j].valeur != -1) && (!gridIsIn(closedList, voisins[j]))){
				if((!gridIsIn(openList, voisins[j]))){
					voisins [j].parent = depart;
					voisins[j].distanceParcourue = voisins [j].calculDepartCourant ();
					voisins[j].distanceVO = voisins [j].volDoiseau (destination.position);
					openList.Add (voisins [j]);

					//VISUEL_____
					//toTest = Instantiate (listeTester [0], voisins[j].position, Quaternion.identity) as GameObject;
					//toTest.transform.SetParent (testGameObject);
					//___________

					if ((voisins[j].position.x == destination.position.x) && (voisins[j].position.y == destination.position.y)) {
						return voisins[j];
					}

				} else {
					int newG = voisins [j].calculDepartCourant ();
					if ((voisins [j].distanceParcourue) > newG) {
						voisins [j].parent = depart;
						voisins[j].distanceParcourue = voisins [j].calculDepartCourant ();
						voisins[j].distanceVO = voisins [j].volDoiseau (destination.position);
					}
				}
			}
		}

		Node nextNode = depart;
		if (openList.Count > 0) {
			nextNode = openList [0];
			for (int n = 1; n < openList.Count; n++) {
				if ((nextNode.distanceParcourue + nextNode.distanceVO >= openList [n].distanceParcourue + openList [n].distanceVO)) {
					nextNode = openList [n];
				}
			}
		}

		if (openList.Count == 1) {
			openList.Remove (nextNode);
			return findPath (destination, nextNode, openList, closedList);
		}
			
		openList.Remove (nextNode);

		if (openList.Count > 0) {
			return findPath (destination, nextNode, openList, closedList);
		}
		else {
			return new Node (1, new Vector2 (-1f, -1f));
		}
	}

	public bool gridIsIn(List<Node> list, Node toTest){
		for (int i = 0; i < list.Count; i++) {
			if((list[i].position.x == toTest.position.x)&&(list[i].position.y == toTest.position.y))
				return true;
		}
		return false;
	}
		
	public List<Node> Voisins(Node HOMME)
	{
		List<Node> voisins = new List<Node>();
		if (HOMME.position.x == 0) {
			voisins.Add (gridPositions [(int)(HOMME.position.x + 1), (int)(HOMME.position.y)]);
		} else if (HOMME.position.x == 14) {
			voisins.Add (gridPositions [(int)(HOMME.position.x - 1), (int)(HOMME.position.y)]);
		} else {
			voisins.Add (gridPositions [(int)(HOMME.position.x + 1), (int)(HOMME.position.y)]);
			voisins.Add (gridPositions [(int)(HOMME.position.x - 1), (int)(HOMME.position.y)]);
		}

		if (HOMME.position.y == 0) {
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y+1)]);
		} else if (HOMME.position.y == 7) {
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y-1)]);
		} else {
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y+1)]);
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y-1)]);
		}

		return voisins;
	}

	public void resetDistanceGrille(){
		for(int y = 0; y < rows; y++){
			for (int x =0; x < columns; x++) {
				gridPositions [x, y].distanceParcourue = 0;
				gridPositions [x, y].distanceVO = 0;
			}
		}
	}

}


