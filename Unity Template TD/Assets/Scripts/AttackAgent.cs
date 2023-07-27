using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using TowerDefense.Towers;
using TowerDefense.Level;
using TowerDefense.Towers.Placement;
using Core.Utilities;
using TowerDefense.UI.HUD;
using Core.Economy;
using System.IO;

public class AttackAgent : Agent
{

    //FileWriter streamWriter = new FileWriter("DefenseAgent", "txt");
    //protected int fileNumber = 1;
    private struct PlacedTowerData
    {
        public int towerType;
        public IntVector2 placementGridCoordinates;
        public int gridTileNumber;
    }

    public List<Agent> UnitsDict;

    [SerializeField] private List<TowerPlacementGrid> placementGrids;
    private TowerPlacementGrid placementArea;


    [SerializeField] private PlayerHomeBase homeBase;
    [SerializeField] private float baseHealth;

    BufferSensorComponent m_BufferSensor;
    private List<PlacedTowerData> m_GridTowerOccupationRepresentative = new List<PlacedTowerData>();

    private Agent unitToSend;
    private int towerIndex;
    private Currency currency;
    private int gridXCoordinateConvertedToContinuousActionScale;
    private int gridYCoordinateConvertedToContinuousActionScale;

    private const int HighestPlacementGridPosition = 14;

    public override void Initialize()
    {

        m_BufferSensor = GetComponent<BufferSensorComponent>();

        LevelManager.instance.resetLose += Loss;
        LevelManager.instance.resetWin += Win;
        LevelManager.instance.homeBases[0].resetbaseHealth += ResetBaseHealth;

        LevelManager.instance.BuildingCompleted();

        currency = LevelManager.instance.currency;
        unitToSend = null;
        UnitsDict = new List<Agent>();
        placementArea = placementGrids[0];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(currency.currentCurrency / 500);
        sensor.AddObservation(homeBase.configuration.currentHealth / baseHealth);
        //Generate for loop while creating a new list of floats for 183 floats
        /*for (int i = 0; i < m_GridTowerOccupationRepresentative.Count; i++)
        {
            float[] listObservation = new float[HighestPlacementGridPosition + 3];

            try
            {
                listObservation[m_GridTowerOccupationRepresentative[i].gridTileNumber] = 1f;
            }
            catch (System.IndexOutOfRangeException)
            {
                Debug.Log("Index" + (m_GridTowerOccupationRepresentative[i].gridTileNumber) + "out of range");
            }
            listObservation[HighestPlacementGridPosition] = m_GridTowerOccupationRepresentative[i].placementGridCoordinates.x / placementGrids[0].dimensions.x - 1;
            listObservation[HighestPlacementGridPosition + 1] = m_GridTowerOccupationRepresentative[i].placementGridCoordinates.y / placementGrids[0].dimensions.y - 1;
            listObservation[HighestPlacementGridPosition + 2] = m_GridTowerOccupationRepresentative[i].towerType / 3f;

            m_BufferSensor.AppendObservation(listObservation);

            //streamWriter.SetStoredData(m_GridTowerOccupationRepresentative[i].gridTileNumber.ToString());
            //streamWriter.SetStoredData(listObservation[HighestPlacementGridPosition].ToString());
            //streamWriter.SetStoredData(listObservation[HighestPlacementGridPosition + 1].ToString());
            //streamWriter.SetStoredData(listObservation[HighestPlacementGridPosition + 2].ToString());
        }*/
    }

    public void Update()
    {
        Debug.Log(homeBase.configuration.currentHealth);
    }
    public override void OnEpisodeBegin()
    {
        if (GameUI.instanceExists) { GameUI.instance.m_CurrentArea = placementArea; }

        ResetBaseHealth();
    }
    public void BuildTower(ActionBuffers actions)
    {
        var discreteTowerTypeSelector = actions.DiscreteActions[0];
        //var discreteplacementGridSelector = actions.DiscreteActions[1];
        var continuousGridXCoordinate = actions.ContinuousActions[0];
        var continuousGridYCoordinate = actions.ContinuousActions[1];

        var tower = UnitsDict[discreteTowerTypeSelector];

        towerIndex = discreteTowerTypeSelector;
        //Generate switch case of discreteTowerTypeSelector
        switch (discreteTowerTypeSelector)
        {
            case 0:
                unitToSend = UnitsDict[0];
                break;
            case 1:
                unitToSend = UnitsDict[1];
                break;
            case 2:
                unitToSend = UnitsDict[2];
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


        gridXCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (Mathf.Abs (continuousGridXCoordinate) * placementArea.dimensions.x - 1);
        gridYCoordinateConvertedToContinuousActionScale = Mathf.RoundToInt (Mathf.Abs (continuousGridYCoordinate) * placementArea.dimensions.y - 1);

        var placementGridCoordinate = new IntVector2(gridXCoordinateConvertedToContinuousActionScale, gridYCoordinateConvertedToContinuousActionScale);

        GameUI.instance.m_GridPosition = placementGridCoordinate;

        m_GridTowerOccupationRepresentative.Add(new PlacedTowerData
        {
            towerType = towerIndex,
            placementGridCoordinates = placementGridCoordinate,
            gridTileNumber = Mathf.Clamp(placementGridCoordinate.x, 1, placementArea.dimensions.x) * Mathf.Clamp(placementGridCoordinate.x, 1, placementArea.dimensions.y) - 1
        });
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

    public void Win()
    {
        SetReward(1f);
        //streamWriter.SetStoredData("\n" + "Won" + "\n");
        //streamWriter = new FileWriter("DefenseAgent" + fileNumber, "txt");
        //fileNumber++;
        m_GridTowerOccupationRepresentative.Clear();
        LevelManager.instance.ResetGame();
        EndEpisode();
    }

    public void Loss()
    {
        SetReward(-1f);
        //streamWriter.SetStoredData("\n" + "Lost" + "\n");
        //streamWriter = new FileWriter("DefenseAgent" + fileNumber, "txt");
        //fileNumber++;
        m_GridTowerOccupationRepresentative.Clear();
        LevelManager.instance.ResetGame();
        EndEpisode();
    }

    public void ResetBaseHealth()
    {         
        homeBase.configuration.SetHealth(baseHealth);
    }

    /*public void OnApplicationQuit()
    {
        streamWriter.WriteString();
    }*/
}
