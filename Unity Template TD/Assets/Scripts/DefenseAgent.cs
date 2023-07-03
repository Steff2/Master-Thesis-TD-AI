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
using TowerDefense.UI.HUD;

public class DefenseAgent : Agent
{

    private Dictionary<int, Tower> towersDictionary;

    [SerializeField] private List<TowerPlacementGrid> placementGrids;
    private TowerPlacementGrid placementArea;

    [SerializeField] private PlayerHomeBase homeBase;

    [SerializeField] private float baseHealth;

    private PoolManager poolManager;
    public override void Initialize()
    {

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);
    }

    public void Awake()
    {
        LevelManager.instance.homeBases[0].diedAgent += OnEpisodeBegin;
        LevelManager.instance.homeBases[0].diedAgent += Lose;
        LevelManager.instance.BuildingCompleted();

        towersDictionary = new Dictionary<int, Tower>();
        placementArea = placementGrids[0];
        if (GameUI.instanceExists) { GameUI.instance.m_CurrentArea = placementArea; }

        var i = 0;
        //Store items in LevelManager.instance.towerLibrary in towersDictionary
        foreach (var tower in LevelManager.instance.towerLibrary)
        {
            towersDictionary.TryAdd(i, tower);
            i++;
        }
    }
    public override void OnEpisodeBegin()
    {
        poolManager = PoolManager.instance;

        //Reset Health of HomeBase
        homeBase.configuration.SetHealth(baseHealth);

        //if (poolManager == null) { Debug.LogError("PoolManager is null"); return; }

        /*foreach (var poolable in poolManager.poolables)
        {
            //Debug.Log("Poolable: " + poolable.name + " is being returned to pool");
            if (poolable.pool != null) { poolManager.ReturnPoolable(poolable); }
        }*/

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


        if (!GameUI.instance.isBuilding) GameUI.instance.SetToBuildMode(towertoPlace);

        var gridXCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (Mathf.Abs (continuousGridXCoordinate) * placementArea.dimensions.x);
        var gridYCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (Mathf.Abs (continuousGridYCoordinate) * placementArea.dimensions.y);

        var placementGridCoordinate = new IntVector2(gridXCoordinateConvertedToContinuousActionScale, gridYCoordinateConvertedToContinuousActionScale);

        GameUI.instance.m_GridPosition = placementGridCoordinate;
        //initialize Tower
        GameUI.instance.BuyTower();
        //GameUI.instance.Unpause();

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

    public void Lose()
    {
        Debug.Log("Lost");
        SetReward(-1f);
        EndEpisode();
    }
}
