using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class FIMScriptableObject : ScriptableObject
{
    public int inputTypes;//number of differentInput
    public int selectedInput;//Input selected
    public List<FIMData> FIMDatas = new List<FIMData>();
}