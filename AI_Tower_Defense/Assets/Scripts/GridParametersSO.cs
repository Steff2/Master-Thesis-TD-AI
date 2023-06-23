using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridParameterSO", menuName = "ScriptableObjects/GridParameterSO")]
public class GridParametersSO : ScriptableObject
{
    public int gridWidth;
    public int gridHeight;
    public float cellSize;
}
