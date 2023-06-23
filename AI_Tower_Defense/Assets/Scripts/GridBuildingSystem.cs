using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{

    [SerializeField] private Transform testTowerTransform;
    [SerializeField] private GridParametersSO gridParameters;

    private Grid<GridObject> grid;

    private void Awake()
    {
        grid = new Grid<GridObject>(gridParameters.gridWidth, gridParameters.gridHeight, gridParameters.cellSize, new Vector3(-50, 0, -18), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;
        public Transform transform;

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void SetTransform(Transform transform)
        {
            this.transform = transform;
        }

        public void ClearTransform()
        {
            transform = null;
        }

        public bool CanBuild()
        {
            return transform == null;
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
            grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

            GridObject gridObject = grid.GetGridObject(x, z);

            if(gridObject.CanBuild())
            {
                Transform builtTransform = Instantiate(testTowerTransform, grid.GetWorldPosition(x, z), Quaternion.identity);
                gridObject.SetTransform(builtTransform);
            }
        }
    }
}
