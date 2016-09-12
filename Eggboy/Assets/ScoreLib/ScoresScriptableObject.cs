using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class ScoresScriptableObject : ScriptableObject
{
    public List<Score> scores = new List<Score>();
}