using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Vector3 worldPositionPoint;
    private Grid grid;
    // Start is called before the first frame update
    private void Start()
    {
        //100 Links/Rechts 40 Hoch/Runter
        grid = new Grid(49, 18, 2f, new Vector3(-50, 0, -18));

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Mouse3D.GetMouseWorldPosition());
            grid.SetValue(Mouse3D.GetMouseWorldPosition(), 40);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log(grid.GetValue(Mouse3D.GetMouseWorldPosition()));
        }
    }
}
