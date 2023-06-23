using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{

    [SerializeField] private GridParametersSO gridParameters;
    [SerializeField] private List<TowerTypeSO> towerTypes;
    private TowerTypeSO towerType;

    private Grid<GridObject> grid;

    private void Awake()
    {
        grid = new Grid<GridObject>(gridParameters.gridWidth, gridParameters.gridHeight, gridParameters.cellSize, new Vector3(-50, 0, -18), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
        towerType = towerTypes[0];
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;
        public PlacedObject placedObject;

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetPlacedObject(PlacedObject placedObject)
        {
            this.placedObject = placedObject;
        }

        public PlacedObject GetPlacedObject()
        {
            return placedObject;
        }
        public void ClearPlacedObject()
        {
            placedObject = null;
        }

        public bool CanBuild()
        {
            return placedObject == null;
        }
        public override string ToString()
        {
            return x + " , " + z;
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Build();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DestroyBuilding();
        }
            /*if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                towerType = towerTypes[1];
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                towerType = towerTypes[2];
            }*/
    }

    private void Build()
    {
        grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

        GridObject gridObject = grid.GetGridObject(x, z);
        if (gridObject == null)
            return;

        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z);

        if (gridObject.CanBuild())
        {
            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, z), towerType);
            gridObject.SetPlacedObject(placedObject);
        }
    }
    private void DestroyBuilding()
    {
        GridObject gridObject = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
        if (gridObject == null)
            return;

        PlacedObject placedObject = gridObject.GetPlacedObject();

        if (placedObject != null)
        {
            placedObject.DestroySelf();
            Vector2Int gridPosition = placedObject.GetGridPosition();
            grid.GetGridObject(gridPosition.x, gridPosition.y).ClearPlacedObject();
        }
    }
}
