using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(Replay))]
public class LevelScriptEditor : Editor
{
    int cibledFileId;

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        Replay myTarget = (Replay)target;
        List<string> ListFilesNames = getFilesNames();
        cibledFileId = ListFilesNames.IndexOf(myTarget.specifiedFileName);
        if(cibledFileId == -1)
        {
            cibledFileId = 0;
        }
        string[] filesNames = ListFilesNames.ToArray();
        if(filesNames.Length == 0)
        {
            return;
        }
        cibledFileId = EditorGUILayout.Popup("Saved Replay", cibledFileId, filesNames);
        myTarget.setSpecifiedFileName(filesNames[cibledFileId]);
        myTarget.gameSeed = int.Parse(filesNames[cibledFileId].Remove(0, filesNames[cibledFileId].Length-10));

    }
    private List<string> getFilesNames()
    {
        List<string> filesNames = new List<string>();
        foreach (string file in System.IO.Directory.GetFiles(Application.dataPath + "/DebugData"))
        {
            if (!file.Contains(".meta"))
            {
                filesNames.Add(file.Remove(0, file.Length - 36).Remove(32, 4));
            }
        }
        return filesNames;
    }
    /*
    private string[] getFilesSeeds()
    {
        List<string> seeds = new List<string>();
        foreach (string file in System.IO.Directory.GetFiles(Application.dataPath + "/DebugData"))
        {
            if (!file.Contains(".meta"))
            {
                seeds.Add(file.Remove(0, file.Length - 14).Remove(10, 4));
            }
        }
        return seeds.ToArray();
    }*/
}