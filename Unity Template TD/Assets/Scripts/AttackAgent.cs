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

    public List<SpawnInstruction> UnitsSpawnInstructionsList;

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

        spawnedAgents = new List<TowerDefense.Agents.Agent> ();

        LevelManager.instance.resetLose += Loss;
        LevelManager.instance.resetWin += Win;
        LevelManager.instance.homeBases[0].resetbaseHealth += ResetBaseHealth;

        currency = LevelManager.instance.currency;
        unitToSend = null;

        Setup();
        //placementArea = placementGrids[0];
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(homeBase.configuration.currentHealth / baseHealth);
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

        switch (discreteTowerTypeSelector)
        {
            case 0:
                unitToSend = UnitsSpawnInstructionsList[0];
                break;
            case 1:
                unitToSend = UnitsSpawnInstructionsList[1];
                break;
            default:
                break;
        }

        if(spawnDelayTimer <= 0)
        {
            if (unitToSend == null) return;

            spawnDelayTimer = spawnDelayTime;

            SpawnAgent(unitToSend.agentConfiguration, unitToSend.startingNode);
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

    protected void Win()
    {
        SetReward(1f);
        //streamWriter.SetStoredData("\n" + "Won" + "\n");
        //streamWriter = new FileWriter("DefenseAgent" + fileNumber, "txt");
        //fileNumber++;
        //m_GridTowerOccupationRepresentative.Clear();
        LevelManager.instance.ResetGame();
        for (int i = 0; i < spawnedAgents.Count; i++)
        {
            spawnedAgents[i].KillAgent();
        }
        EndEpisode();
    }

    protected void Loss()
    {
        SetReward(-1f);
        //streamWriter.SetStoredData("\n" + "Lost" + "\n");
        //streamWriter = new FileWriter("DefenseAgent" + fileNumber, "txt");
        //fileNumber++;
        //m_GridTowerOccupationRepresentative.Clear();
        LevelManager.instance.ResetGame();
        for(int i = 0; i < spawnedAgents.Count; i++)
        {
            spawnedAgents[i].KillAgent();
        }
        EndEpisode();
    }

    public void ResetBaseHealth()
    {         
        homeBase.configuration.SetHealth(baseHealth);
    }

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

    //Create the layout for the towers for training
    protected void Setup()
    {
        
    }
}


