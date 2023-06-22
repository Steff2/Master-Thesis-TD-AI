using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Vector3 worldPositionPoint;
    private Grid<GridObject> grid;
    // Start is called before the first frame update
    private void Start()
    {
        //100 Links/Rechts 40 Hoch/Runter
        grid = new Grid<GridObject>(20, 10, 3f, new Vector3(-50, 0, -18), (Grid<GridObject> g, int x, int z) => new GridObject(g, x, z));

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Mouse3D.GetMouseWorldPosition();
            GridObject gridObject = grid.GetGridObject(position);
            if(gridObject != null)
            {
                gridObject.AddValue(5);
                grid.SetGridObject(position, gridObject);
            }
        }
    }

    public class GridObject
    {
        private const int MIN = 0;
        private const int MAX = 100;

        private int value;
        private Grid<GridObject> grid;
        private int x;
        private int z;

        public GridObject(Grid<GridObject> grid, int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }

        public void AddValue(int addValue)
        {
            value += Mathf.Clamp(addValue, MIN, MAX);
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
