using UnityEngine;
using System.Collections;
using System;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        print(Input.GetJoystickNames()[0]);
        if (FIM.check("Jump"))
        {
            print("Jump");
        }
        if (FIM.check("Run"))
        {
            print("Run");
        }
        detectPressedKeyOrButton();
    }
    public void detectPressedKeyOrButton()
    {
        foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(kcode))
                Debug.Log("KeyCode down: " + kcode);
        }
    }
}

