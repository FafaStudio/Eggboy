using UnityEngine;
using System.Collections;

public class SingleTon : MonoBehaviour {

	public static SingleTon instance = null; 

	void Awake(){
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);    

		//Permet de ne pas détruire en changeant de level
		DontDestroyOnLoad(gameObject);
	}
}
