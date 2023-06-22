using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private Vector3 worldPositionPoint;
    private Grid<bool> grid;
    // Start is called before the first frame update
    private void Start()
    {
        //100 Links/Rechts 40 Hoch/Runter
        grid = new Grid<bool>(20, 10, 3f, new Vector3(-50, 0, -18));

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 position = Mouse3D.GetMouseWorldPosition();
            grid.SetValue(position, true);
        }
    }
}
