using UnityEngine;
using System.Collections;

public class FIM : MonoBehaviour {



    public static bool check(string FIMName)
    {
        string selectedInput = PlayerPrefs.GetString("FIMSelectedInput");
        return Input.GetKeyDown((KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("FIM"+ FIMName + selectedInput)));
    }
}
