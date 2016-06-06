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

	public class Grid
	{
		public int valeur = 0;
		public Vector2 position;
		public Grid parent;
		public int distanceParcourue;
		public int distanceVO;

		public Grid(int x, Vector2 p){
			this.valeur = x;
			this.position = p;
			this.parent = null;
			distanceParcourue = 0;
			distanceVO = 0;
		}

		public Grid(){}

		public void setValeur(int value){
			this.valeur = value;
		}

		public String toString(){
			return " [" + this.valeur.ToString () + "] ";
		}

		public int volDoiseau(Vector2 destination)
		{
			return ((int)(Mathf.Abs ((destination.y - this.position.y) + (destination.x - this.position.x))));
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
	public toInstantiate[] objectTiles;

	private Transform boardHolder;
	//boardHolder permet lorsqu'on fait spawn plein de trucs de garder la hierarchie clean
	private Transform objectInLevel;

	private Grid[,] gridPositions;

	public GameObject[] listeTester;
	public Transform testGameObject;


	void InitialiseGrille(){
		gridPositions = new Grid[columns, rows];
		for(int y = rows-1; y >= 0; y--){
			for (int x =0; x < columns; x++) {
				gridPositions[x,y] = new Grid(1,new Vector2(x,y));
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

	public void setCellOnGrid(int x, int y , int value){
		gridPositions [x, y].setValeur(value);
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

	void InitialiseLevelDesign(){
		objectInLevel = new GameObject ("ObjectInLevel").transform;
		for(int i = 0; i < objectTiles.Length; i++){
			Vector3 position = new Vector3 (objectTiles [i].x, objectTiles [i].y, 0f);
			GameObject instantiate = Instantiate (objectTiles [i].objectInstantiate, position, Quaternion.identity) as GameObject;
			instantiate.transform.SetParent (objectInLevel);
		}
	}

	public void SetupScene(int level){
		boardSetup ();
		InitialiseGrille ();
		InitialiseLevelDesign ();

		testGameObject = new GameObject ("testGameObject").transform;
	}

	public Vector2 doPathfinding (Grid destination, Grid depart){
		Grid path = findPath (destination, depart);

		GameObject toTest = Instantiate (listeTester [2], path.position, Quaternion.identity) as GameObject;
		toTest.transform.SetParent (testGameObject);

		if (path.parent != null) {
			while (path.parent.parent != null) {
				path = path.parent;

				toTest = Instantiate (listeTester [2], path.position, Quaternion.identity) as GameObject;
				toTest.transform.SetParent (testGameObject);
			}	
		}
		toTest = Instantiate (listeTester [2], path.parent.position, Quaternion.identity) as GameObject;
		toTest.transform.SetParent (testGameObject);

		resetDistanceGrille ();
		return path.position;
	}

	public Grid findPath(Grid destination, Grid depart){

		testGameObject = new GameObject ("testGameObject").transform;

		List<Grid> openList = new List<Grid> ();
		List<Grid> closedList = new List<Grid> ();

		openList.Add (depart);
		Grid nextGrid = openList [0];
		nextGrid.distanceParcourue = nextGrid.calculDepartCourant ();
		nextGrid.distanceVO = nextGrid.volDoiseau (destination.position);

		for(int i =0; i < openList.Count; i++){
			
			nextGrid = openList [0];
			for(int n = 1; n < openList.Count; n++){
				if (nextGrid.distanceParcourue + nextGrid.distanceVO >= openList [n].distanceParcourue + openList [n].distanceVO) {
					nextGrid = openList [n];
				}
			}

			closedList.Add (nextGrid);

			GameObject toTest = Instantiate (listeTester [1], nextGrid.position, Quaternion.identity) as GameObject;
			toTest.transform.SetParent (testGameObject);

			if ((nextGrid.position.x == destination.position.x) && (nextGrid.position.y == destination.position.y)) {
				return nextGrid;
			}

			openList.Remove (nextGrid);
			i=0;

			List<Grid> voisins = Voisins (nextGrid);

			for (int j = 0; j < voisins.Count; j++) {
				if ((voisins [j].valeur != -1) && (!gridIsIn(closedList, voisins[j]))){
					if((!gridIsIn(openList, voisins[j]))){
						voisins [j].parent = nextGrid;
						voisins[j].distanceParcourue = voisins [j].calculDepartCourant ();
						voisins[j].distanceVO = voisins [j].volDoiseau (destination.position);
						openList.Add (voisins [j]);

						toTest = Instantiate (listeTester [0], voisins[j].position, Quaternion.identity) as GameObject;
						toTest.transform.SetParent (testGameObject);

					} else {
						int newG = voisins [j].calculDepartCourant ();
						if ((voisins [j].distanceParcourue) > newG) {
							voisins [j].parent = nextGrid;
							voisins[j].distanceParcourue = voisins [j].calculDepartCourant ();
							voisins[j].distanceVO = voisins [j].volDoiseau (destination.position);
						}
					}
				}
			}
		}
		return new Grid(1, new Vector2(-1f, -1f));
	}

	public bool gridIsIn(List<Grid> list, Grid toTest){
		for (int i = 0; i < list.Count; i++) {
			if((list[i].position.x == toTest.position.x)&&(list[i].position.y == toTest.position.y))
				return true;
		}
		return false;
	}
		
	public List<Grid> Voisins(Grid HOMME)
	{
		List<Grid> voisins = new List<Grid>();
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


