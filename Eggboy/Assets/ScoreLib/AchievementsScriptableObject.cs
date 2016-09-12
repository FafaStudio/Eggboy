using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class AchievementsScriptableObject : ScriptableObject
{
    public List<Achievement> achievements = new List<Achievement>();
}