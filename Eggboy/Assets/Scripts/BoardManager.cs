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
		public void setValeur(int value){
			this.valeur = value;
		}

		public String toString(){
			return " [" + this.valeur.ToString () + "] ";
		}

		public void volDoiseau(Vector2 destination)
		{
			this.distanceVO = (int)(Mathf.Abs ((destination.y - this.position.y) + (destination.x - this.position.x)));
		}

		public void calculDepartCourant()
		{
			if (this.parent == null) {
				this.distanceParcourue = 0;
			} else {
				this.parent.calculDepartCourant();
				this.distanceParcourue = this.valeur + this.parent.distanceParcourue;
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





	void InitialiseList(){
		gridPositions = new Grid[columns, rows];
		for (int x =0; x < columns; x++) {
			for(int y = 0; y < rows; y++){
				gridPositions[x,y] = new Grid(1,new Vector2(x,y));
			/*	if (x == columns - 1)
					print (gridPositions [x, y].toString () + "/n");
				else
					print (gridPositions [x, y].toString ());*/
			}
		}
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
		InitialiseList ();
		InitialiseLevelDesign ();
	}

	public List<Grid> findPath(Grid destination, Grid depart, List<Grid> openList, List<Grid> closedList)
	{
		openList.Add (depart);

		Grid current = null;
		Grid finalCurrent = openList [0];

		for (int i = 0; i < openList.Count; i++) {
			current = openList [i];
			current.volDoiseau (destination.position);
			current.calculDepartCourant ();
			finalCurrent.calculDepartCourant ();
			finalCurrent.volDoiseau (destination.position);
			if ((current.distanceParcourue + current.distanceVO) < (finalCurrent.distanceParcourue + finalCurrent.distanceVO)) {
				finalCurrent = current;
			}
		}

		closedList.Add (finalCurrent);
		if (finalCurrent.position == destination.position) {
			return closedList;
		}

		List<Grid> voisins = Voisins (finalCurrent);

		for (int i = 0; i < voisins.Count; i++) {
				if ((voisins [i].valeur != -1) && !closedList.Contains (voisins [i])) {
					if (!openList.Contains (voisins [i])) {
						voisins [i].parent = finalCurrent;
						voisins [i].calculDepartCourant ();
						voisins [i].volDoiseau (destination.position);
						openList.Add (voisins [i]);
					} else {
						int g = voisins [i].distanceParcourue + voisins[i].distanceVO;
						voisins [i].calculDepartCourant ();
						voisins [i].volDoiseau (destination.position);

						if ((voisins [i].distanceParcourue + voisins [i].distanceVO) < g) {
							voisins [i].parent = finalCurrent;
							voisins [i].calculDepartCourant ();
							voisins [i].volDoiseau (destination.position);
						}

					}
				}
				
			}
		return openList;
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
		} else if (HOMME.position.x == 7) {
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y-1)]);
		} else {
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y+1)]);
			voisins.Add (gridPositions [(int)(HOMME.position.x), (int)(HOMME.position.y-1)]);
		}

		return voisins;
	}

}


