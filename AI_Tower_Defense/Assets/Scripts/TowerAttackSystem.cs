using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttackSystem : MonoBehaviour
{
    [SerializeField] private TowerTypeSO towerType;

    private Vector3 positionToShootProjectileFrom;

    private void Awake()
    {
        positionToShootProjectileFrom = transform.Find("ProjectileShootFromPosition").position;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ProjectileArrow.Create(towerType, positionToShootProjectileFrom, Mouse3D.GetMouseWorldPosition());
        }
    }
}
