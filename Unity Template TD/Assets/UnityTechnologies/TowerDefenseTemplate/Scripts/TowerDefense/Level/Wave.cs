﻿using System;
using System.Timers;
using System.Collections.Generic;
using Core.Extensions;
using Core.Utilities;
using TowerDefense.Agents;
using TowerDefense.Agents.Data;
using TowerDefense.Nodes;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// A Wave is a TimedBehaviour, that uses the RepeatingTimer to spawn enemies
	/// </summary>
	public class Wave : TimedBehaviour
	{
		/// <summary>
		/// A list of instructions on how to spawn enemies
		/// </summary>
		public List<SpawnInstruction> spawnInstructions;

		protected List<Agent> spawnedAgents;

		/// <summary>
		/// The index of the current enemy to spawn
		/// </summary>
		protected int m_CurrentIndex;

		/// <summary>
		/// The RepeatingTimer used to spawn enemies
		/// </summary>
		protected RepeatingTimer m_SpawnTimer;

		/// <summary>
		/// The event that is fired when a Wave is completed
		/// </summary>
		public event Action waveCompleted;

		public virtual float progress
		{
			get { return (float) (m_CurrentIndex) / spawnInstructions.Count; }
		}

		public int waveNumber;

		private static System.Timers.Timer spawnTimer;
		/// <summary>
		/// Initializes the Wave
		/// </summary>
		public virtual void Init()
		{
			spawnTimer = new System.Timers.Timer(2000);

			spawnedAgents = new List<Agent>();

            // If the wave is empty then warn the level designer and fire complete event
            if (spawnInstructions.Count == 0)
			{
				Debug.LogWarning("[LEVEL] Empty Wave");
				SafelyBroadcastWaveCompletedEvent();
				return;
			}
            //m_SpawnTimer = new RepeatingTimer(spawnInstructions[0].delayToSpawn, SpawnCurrent);
            //StartTimer(m_SpawnTimer);

			spawnTimer.Elapsed += SpawnCurrent;
			spawnTimer.AutoReset = false;
			spawnTimer.Enabled = true;
			spawnTimer.Start();

		}

        /// <summary>
        /// Handles spawning the current agent and sets up the next agent for spawning
        /// </summary>
        protected virtual void SpawnCurrent(System.Object source, ElapsedEventArgs e)
		{
			Spawn();
			if (!TrySetupNextSpawn())
			{
				SafelyBroadcastWaveCompletedEvent();
				// this is required so wave progress is still accurate
				m_CurrentIndex = spawnInstructions.Count;
				//StopTimer(m_SpawnTimer);

				spawnTimer.Stop();
			}
		}

		/// <summary>
		/// Spawns the current agent
		/// </summary>
		protected void Spawn()
		{
			if(m_CurrentIndex < spawnInstructions.Count)
			{
                SpawnInstruction spawnInstruction = spawnInstructions[m_CurrentIndex];
                SpawnAgent(spawnInstruction.agentConfiguration, spawnInstruction.startingNode);
            }

		}

		/// <summary>
		/// Tries to setup the next spawn
		/// </summary>
		/// <returns>true if there is another spawn instruction, false if not</returns>
		protected bool TrySetupNextSpawn()
		{
			bool hasNext = spawnInstructions.Next(ref m_CurrentIndex);
			if (hasNext)
			{
				//Generate console message for wave number + m_CurrentIndex
				Debug.Log("Wave " + waveNumber + " SpawnIndex: " + m_CurrentIndex);
				SpawnInstruction nextSpawnInstruction = spawnInstructions[m_CurrentIndex];
				/*if (nextSpawnInstruction.delayToSpawn <= 0f)
				{
					SpawnCurrent();
				}
				else
				{
					m_SpawnTimer.SetTime(nextSpawnInstruction.delayToSpawn);
				}*/
			}

			return hasNext;
		}

		/// <summary>
		/// Spawns the agent
		/// </summary>
		/// <param name="agentConfig">The agent to spawn</param>
		/// <param name="node">The starting node that the agent uses</param>
		protected virtual void SpawnAgent(AgentConfiguration agentConfig, Node node)
		{
			Vector3 spawnPosition = node.GetRandomPointInNodeArea();

			var poolable = Poolable.TryGetPoolable<Poolable>(agentConfig.agentPrefab.gameObject);
			if (poolable == null)
			{
				return;
			}
			var agentInstance = poolable.GetComponent<Agent>();
			agentInstance.transform.position = spawnPosition;
			agentInstance.Initialize();
			agentInstance.SetNode(node);
			agentInstance.transform.rotation = node.transform.rotation;
            spawnedAgents.Add(agentInstance);

        }

        /// <summary>
        /// Launch the waveCompleted event
        /// </summary>
        protected void SafelyBroadcastWaveCompletedEvent()
		{
            waveCompleted?.Invoke();
        }

		public virtual void ResetWave()
		{
			//Generate for loop to destroy all agents in spawnedAgents
			for (int i = 0; i < spawnedAgents.Count; i++)
			{
				spawnedAgents[i].KillAgent();
			}

            /*if (m_SpawnTimer != null)
            {
                StopTimer(m_SpawnTimer);
            }*/

			if (spawnTimer != null)
			{
                spawnTimer.Stop();
            }
            m_CurrentIndex = 0;
			waveNumber = 1;
        }

	}
}