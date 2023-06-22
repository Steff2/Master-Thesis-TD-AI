using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{

    [SerializeField] private Transform testTowerTransform;

    private Grid<GridObject> grid;

    private void Awake()
    {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));
    }
    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
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
            Instantiate(testTowerTransform, grid.GetWorldPosition(x, z), Quaternion.identity);
        }
    }
}
