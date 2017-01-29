using UnityEngine;
using UnityEditor;
using System.Collections;

public class FIMWindow : EditorWindow {
    [MenuItem("FafaStudio/FafasInputManager/savePlayerPref")]
    public static void test()
    {
        FIMScriptableObject fimScriptableObject = Resources.Load("FIMScriptableObject") as FIMScriptableObject;
        PlayerPrefs.SetString("FIMInputTypes", fimScriptableObject.inputTypes.ToString());
        PlayerPrefs.SetString("FIMSelectedInput", fimScriptableObject.selectedInput.ToString());
        /*
        for (int i = 0; i < fimScriptableObject.FIMDatas.Count; i++)
        {
            for (int j = 0; j < fimScriptableObject.inputTypes; j++) {
                PlayerPrefs.SetString("FIM" + fimScriptableObject.FIMDatas[i].name + j, fimScriptableObject.FIMDatas[i].inputs[j]);
            }
        }*/
    }

    [MenuItem("FafaStudio/FafasInputManager/InputManager")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(FIMWindow));
    }
    int tab;
    void OnGUI()
    {
        tab = GUILayout.Toolbar(tab, new string[] { "Inputs" , "InputTypes", "InputList"});

        switch (tab)
        {
            case 0:
                break;
            case 1:
                inputTypes();
                break;
            case 2:
                inputList();
                break;
            default:
                break;
        }
    }

    private void inputTypes()
    {
        FIMScriptableObject fimScriptableObject = AssetDatabase.LoadAssetAtPath<FIMScriptableObject>("Assets/FafasInputManager/FIMScriptableObject.asset");
        EditorGUILayout.BeginVertical();

        string[] listInput = Input.GetJoystickNames();

        for (int i = 0; i < listInput.Length; i++)
        {
            EditorGUILayout.LabelField("Connected Joysticks :" + listInput[i]);
        }


        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < fimScriptableObject.inputTypes.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            fimScriptableObject.inputTypes[i] = EditorGUILayout.TextArea(fimScriptableObject.inputTypes[i], GUILayout.Width(150));
            if (GUILayout.Button("Remove"))
            {
                fimScriptableObject.inputTypes.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add input"))
        {
            fimScriptableObject.inputTypes.Add("");
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(fimScriptableObject);
        }
        EditorGUILayout.EndVertical();
    }

    private void inputList()
    {
        FIMScriptableObject fimScriptableObject = AssetDatabase.LoadAssetAtPath<FIMScriptableObject>("Assets/FafasInputManager/FIMScriptableObject.asset");
        EditorGUILayout.BeginVertical();


        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < fimScriptableObject.inputNames.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            fimScriptableObject.inputNames[i] = EditorGUILayout.TextArea(fimScriptableObject.inputNames[i], GUILayout.Width(150));
            if (GUILayout.Button("Remove"))
            {
                fimScriptableObject.inputNames.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add input"))
        {
            fimScriptableObject.inputNames.Add("");
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(fimScriptableObject);
        }
        EditorGUILayout.EndVertical();
    }


}
