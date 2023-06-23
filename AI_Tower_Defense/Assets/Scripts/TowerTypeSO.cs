using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerTypeScriptableObject", menuName = "ScriptableObjects/TowerTypeScriptableObject")]
public class TowerTypeSO : ScriptableObject
{
    public enum DamageType
    {
        SingleTarget,
        AreaDamage
    }

    public string nameString;
    public Transform prefab;
    public Transform visuals;
    public int width;
    public int height;
    public int damage;
    public Transform projectile;
    public DamageType damageType;
}
