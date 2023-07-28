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
using TowerDefense.Agents.Data;
using TowerDefense.Nodes;
using TowerDefense.Agents;

public class AttackAgent : Unity.MLAgents.Agent
{

    //FileWriter streamWriter = new FileWriter("DefenseAgent", "txt");
    //protected int fileNumber = 1;
    private struct PlacedTowerData
    {
        public int towerType;
        public IntVector2 placementGridCoordinates;
        public int gridTileNumber;
    }

    public List<SpawnInstruction> UnitsSpawnInstructionsDict;

    [SerializeField] private List<TowerPlacementGrid> placementGrids;
    private TowerPlacementGrid placementArea;
    protected List<TowerDefense.Agents.Agent> spawnedAgents;



    [SerializeField] private PlayerHomeBase homeBase;
    [SerializeField] private float baseHealth;

    BufferSensorComponent m_BufferSensor;
    private List<PlacedTowerData> m_GridTowerOccupationRepresentative = new List<PlacedTowerData>();

    private SpawnInstruction unitToSend;
    private Currency currency;


    private const int HighestPlacementGridPosition = 14;

    private const int spawnDelayTime = 2;
    private float spawnDelayTimer = 0;

    public override void Initialize()
    {
        spawnDelayTimer = spawnDelayTime;
        m_BufferSensor = GetComponent<BufferSensorComponent>();

        LevelManager.instance.resetLose += Loss;
        LevelManager.instance.resetWin += Win;
        LevelManager.instance.homeBases[0].resetbaseHealth += ResetBaseHealth;

        LevelManager.instance.BuildingCompleted();

        currency = LevelManager.instance.currency;
        unitToSend = null;
        UnitsSpawnInstructionsDict = new List<SpawnInstruction>();
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
        if (spawnDelayTimer > 0)
        {
            spawnDelayTimer -= Time.deltaTime;
        }

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

        //Generate switch case of discreteTowerTypeSelector
        switch (discreteTowerTypeSelector)
        {
            case 0:
                unitToSend = UnitsSpawnInstructionsDict[0];
                break;
            case 1:
                unitToSend = UnitsSpawnInstructionsDict[1];
                break;
            case 2:
                unitToSend = UnitsSpawnInstructionsDict[2];
                break;
            default:
                break;
        }

        if(spawnDelayTimer <= 0)
        {
            if (unitToSend == null) return;

            SpawnAgent(unitToSend.agentConfiguration, unitToSend.startingNode);

            spawnDelayTimer = spawnDelayTime;
        }
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
    protected virtual void SpawnAgent(AgentConfiguration agentConfig, Node node)
    {
        Vector3 spawnPosition = node.GetRandomPointInNodeArea();

        var poolable = Poolable.TryGetPoolable<Poolable>(agentConfig.agentPrefab.gameObject);
        if (poolable == null)
        {
            return;
        }
        var agentInstance = poolable.GetComponent<TowerDefense.Agents.Agent>();
        agentInstance.transform.position = spawnPosition;
        agentInstance.Initialize();
        agentInstance.SetNode(node);
        agentInstance.transform.rotation = node.transform.rotation;
        spawnedAgents.Add(agentInstance);
    }
}


