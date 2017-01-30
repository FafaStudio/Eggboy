using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	private float decreaseFactor = 1.0f;
	private float shake = 0f;
	private float shakeAmount = 0.1f;
	private Camera camera;

	void Start(){
		camera = this.GetComponent<Camera> ();
	}

	void Update() {
		if (shake > 0) {
			camera.transform.position = Random.insideUnitSphere * shakeAmount + new Vector3 (7f, 3.75f, -10f);
			shake -= Time.deltaTime * decreaseFactor;

		} else {
			shake = 0.0f;
			camera.gameObject.transform.position = new Vector3 (7f, 3.75f, -10f);
		}
	}

	public void setShake(float value){
		shake = value;
	}
}
