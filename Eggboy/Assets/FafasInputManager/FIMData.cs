using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FIMData
{
    [SerializeField]
    public string catégorie;
    [SerializeField]
    public string name;
    [SerializeField]
    public List<string> inputs;
}
