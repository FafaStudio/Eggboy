using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class FIMScriptableObject : ScriptableObject
{
    public List<string> inputTypes;
    public List<string> inputNames;
    public int selectedInput;//Input selected
    public List<FIMData> FIMDatas = new List<FIMData>();
}