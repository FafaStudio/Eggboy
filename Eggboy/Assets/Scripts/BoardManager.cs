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
		public GameObject nodeObject = null;
		//joueur, ennemis, murs
		// pas les lasers, roquettes, pièges

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

		public void setObject(GameObject setter){
			nodeObject = setter;
		}

		public void resetCharacter(){
			nodeObject = null;
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

	public Node[,] gridPositions;

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
				if (x == columns - 1) {
					if (gridPositions [x, y].valeur == 1) {
						toString += "_" + gridPositions [x, y].toString () + " \n";
					} else {
						toString += gridPositions [x, y].toString () + " \n";
					}
				} else {
					if (gridPositions [x, y].valeur == 1) {
						toString += "_" + gridPositions [x, y].toString ();
					} else {
						toString += gridPositions [x, y].toString ();
					}
				}
			}
		}
		return toString;
	}

	public String grilleToStringWithObjects(){
		String toString = " ";
		for(int y = rows-1; y >= 0; y--){
			for (int x =0; x < columns; x++) {
				if (x == columns - 1) {
					if(gridPositions [x, y].nodeObject!=null)
						toString += gridPositions [x, y].nodeObject.gameObject.name.ToString () + " \n";
					else
						toString += "[null]" + " \n";
				} else {
					if(gridPositions [x, y].nodeObject!=null)
						toString += gridPositions [x, y].nodeObject.gameObject.name.ToString ();
					else
						toString += "[null]";
				}
			}
		}
		return toString;
	}

	public void setNodeOnGrid(int x, int y , int value){
		gridPositions [x, y].setValeur(value);
	}

	public void setNodeOnGrid(int x, int y , int value, Trap piege){
		gridPositions [x, y].setValeur(value);
		gridPositions [x, y].setPiege (piege);
	}

	public void setObjectOnGrid(int x, int y , int value, GameObject nodeObject){
	// setter de la grille lors d'un déplacement d'un personnage
		gridPositions [x, y].setValeur(value);
		gridPositions [x, y].setObject (nodeObject);
	}

	public GameObject testCaseCharacterPiege(int x, int y){
	// detecte un personage sur une case sans autre info que la coordonnée
		if (gridPositions [x, y].nodeObject != null) 
			return gridPositions [x, y].nodeObject;
		else
			return null;
	}

	public void testCasePiege(MovingObject persoToTest){
	// teste une case piégé a partir de la position d'un personnage
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
//PATHFINDING A*_____________________________________________________________________________________________________

	public Vector2 doPathfinding (Node destination, Node depart){
		Node path = findPath (destination, depart, new List<Node>(), new List<Node>());
		if (path.parent != null) {
			while (path.parent.parent != null) {
				path = path.parent;
			}	
		}
		resetDistanceGrille ();
		return path.position;
	}

	public Node findPath(Node destination, Node depart, List<Node> openList, List<Node> closedList){
		closedList.Add (depart);

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
		
	public List<Node> Voisins(Node HOMME){
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

//TRI RAPIDE POUR LA LISTE DES ENNEMIS__________________________________________________________________________
//pour que les ennemis joue dans l'ordre croissante de leur distance au joueur
	List<Enemy> enemiesList;

	void Start(){
		enemiesList = GameManager.instance.enemies;
	}

	public void launchQuickSort(){
		BoardManager.Node target = enemiesList [0].getTarget ();
		//print (enemiesList [0].getTarget ().position);
		foreach (Enemy enemy in enemiesList) {
			enemy.caseExacte.distanceVO = totalDistanceWithPathfinding (target, enemy.caseExacte);
			if (enemy.caseExacte.distanceVO == 0)
				//si jamais le joueur est innaccessible
				enemy.caseExacte.distanceVO = enemy.caseExacte.volDoiseau (target.position);
		}
		quicksort (enemiesList,0, enemiesList.Count-1);
		foreach (Enemy enemy in enemiesList) {
			/*print ("________________");
			print (enemy.name);
			print (enemy.caseExacte.position);
			print (enemy.caseExacte.distanceVO);
			print ("________________");*/
		}
		resetDistanceGrille ();
	}
		
	public int pivot(int left, int right){
		int pivot =enemiesList[left].caseExacte.distanceVO;
		while (left<enemiesList.Count)
		{
			while (enemiesList[left].caseExacte.distanceVO < pivot)
				left++;
			while (enemiesList[right].caseExacte.distanceVO > pivot)
				right--;
			if (enemiesList[left].caseExacte.distanceVO == pivot && enemiesList[right].caseExacte.distanceVO == pivot)
				left++;

			if (left < right)
			{
				Enemy temp = enemiesList[right];
				enemiesList[right] = enemiesList[left];
				enemiesList[left] = temp;
			}
			else
			{
				return right;
			}
		}
		return right;
	}

	public void quicksort(List<Enemy> enemies, int i, int j){
	//i et j permettent de sauvegarder les indices lors de la recursivite
		if (j - i > 0){
			int piv = pivot (i,j);
			quicksort (enemies, i, piv - 1);
			quicksort (enemies, piv + 1, j);
		}
	}

	public int totalDistanceWithPathfinding(Node destination, Node depart){
		int result = 0;
		Node path = findPathWithEnemies (destination, depart, new List<Node>(), new List<Node>());

		if (path.parent != null) {
			result++;
			/*while (path.parent != null) {
				result++;
				path = path.parent;
				print (path.position);
			}	*/
			while (path.position != depart.position) {
				result++;
				path = path.parent;
			}
		}
		return result;
	}

	public Node findPathWithEnemies(Node destination, Node depart, List<Node> openList, List<Node> closedList){
	//pour prendre en compte la case des enemies lors de lexecution du quicksort au debut du tour 
		closedList.Add (depart);

		if ((depart.position.x == destination.position.x) && (depart.position.y == destination.position.y)) {
			return depart;
		}
		List<Node> voisins = Voisins (depart);
		for (int j = 0; j < voisins.Count; j++) {
			if (((voisins [j].valeur != -1)) 
				&& (!gridIsIn(closedList, voisins[j]))){
				if((!gridIsIn(openList, voisins[j]))){
					voisins [j].parent = depart;
					voisins[j].distanceParcourue = voisins [j].calculDepartCourant ();
					voisins[j].distanceVO = voisins [j].volDoiseau (destination.position);
					openList.Add (voisins [j]);

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
}


