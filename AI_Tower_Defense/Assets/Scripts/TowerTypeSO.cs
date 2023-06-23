using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerTypeScriptableObject", menuName = "ScriptableObjects/TowerTypeScriptableObject")]
public class TowerTypeSO : ScriptableObject
{
    public string nameString;
    public Transform prefab;
    public int width;
    public int height;
}
