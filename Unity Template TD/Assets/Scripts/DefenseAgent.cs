using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TowerDefense.Towers;
using TowerDefense.Level;
using TowerDefense.Towers.Placement;
using Core.Utilities;

public class DefenseAgent : Agent
{

    private Dictionary<int, Tower> towersDictionary;
    [SerializeField] private List<TowerPlacementGrid> placementGrids;
    [SerializeField] private IPlacementArea placementArea;

    private PoolManager poolManager;
    public override void Initialize()
    {
        LevelManager.instance.homeBases[0].diedAgent += OnEpisodeBegin;

        poolManager = PoolManager.instance;

        var i = 0;
        //Store items in LevelManager.instance.towerLibrary in towersDictionary
        foreach (var tower in LevelManager.instance.towerLibrary)
        {
            towersDictionary.TryAdd(i, tower);
            i++;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public override void OnEpisodeBegin()
    {
        //Generate For loop for poolable in poolManager.poolables
        //poolManager.ReturnPoolable(poolable);
        foreach (var poolable in poolManager.poolables)
        {
            poolable.pool.ReturnAll();
        }

    }

    public void BuildTower(ActionBuffers actions)
    {
        var discreteTowerTypeSelector = actions.DiscreteActions[0];
        //var discreteplacementGridSelector = actions.DiscreteActions[1];
        var continuousGridXCoordinate = actions.ContinuousActions[0];
        var continuousGridYCoordinate = actions.ContinuousActions[1];

        var tower = towersDictionary[actions.DiscreteActions[0]];

        Tower towertoPlace = null;
        //Generate switch case of discreteTowerTypeSelector
        switch (discreteTowerTypeSelector)
        {
            case 0:
                towertoPlace = towersDictionary[0];
                break;
            case 1:
                towertoPlace = towersDictionary[1];
                break;
            case 2:
                towertoPlace = towersDictionary[2];
                break;
            default:
                break;
        }

        /*TowerPlacementGrid placementGrid = null;
        //Generate switch case of discreteplacementGridSelector
        switch (discreteplacementGridSelector)
        {
            case 0:
                placementGrid = placementGrids[0];
                break;
            case 1:
                placementGrid = placementGrids[1];
                break;
            case 2:
                placementGrid = placementGrids[2];
                break;
            default:
                break;
        }*/

        var gridXCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (continuousGridXCoordinate / placementGrids[0].dimensions.x);
        var gridYCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (continuousGridYCoordinate / placementGrids[0].dimensions.y);

        var placementGridCoordinate = new IntVector2(gridXCoordinateConvertedToContinuousActionScale, gridYCoordinateConvertedToContinuousActionScale);

        //initialize Tower
        towertoPlace.Initialize(placementArea, placementGridCoordinate);


    }

    public override void OnActionReceived(ActionBuffers actions)
    {


        BuildTower(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        base.Heuristic(actionsOut);
    }

    public void Reset()
    {
        //Base need to be reset
    }
}
