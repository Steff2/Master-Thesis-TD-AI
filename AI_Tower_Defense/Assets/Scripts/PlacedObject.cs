using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, TowerTypeSO towertype)
    {
        Transform placedObjectTransform = Instantiate(towertype.prefab, worldPosition, Quaternion.identity);

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        placedObject.towerType = towertype;
        placedObject.origin = origin;

        return placedObject;
    }

    private TowerTypeSO towerType;
    private Vector2Int origin;

    public Vector2Int GetGridPosition()
    {
        return origin;
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
