using UnityEngine;
using UnityEditor;
using System.Collections;

public class FIMWindow : EditorWindow {
    [MenuItem("FafaStudio/FafasInputManager/savePlayerPref")]
    public static void ShowWindow()
    {
        FIMScriptableObject fimScriptableObject = Resources.Load("FIMScriptableObject") as FIMScriptableObject;
        PlayerPrefs.SetString("FIMInputTypes", fimScriptableObject.inputTypes.ToString());
        PlayerPrefs.SetString("FIMSelectedInput", fimScriptableObject.selectedInput.ToString());
        for (int i = 0; i < fimScriptableObject.FIMDatas.Count; i++)
        {
            for (int j = 0; j < fimScriptableObject.inputTypes; j++) {
                PlayerPrefs.SetString("FIM" + fimScriptableObject.FIMDatas[i].name + j, fimScriptableObject.FIMDatas[i].inputs[j]);
            }
        }
    }
}
