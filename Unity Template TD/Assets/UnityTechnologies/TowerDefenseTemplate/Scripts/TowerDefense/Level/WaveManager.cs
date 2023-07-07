using System;
using System.Collections.Generic;
using Core.Extensions;
using UnityEngine;

namespace TowerDefense.Level
{
	/// <summary>
	/// WaveManager - handles wave initialisation and completion
	/// </summary>
	public class WaveManager : MonoBehaviour
	{
		/// <summary>
		/// Current wave being used
		/// </summary>
		protected int m_CurrentIndex;

		/// <summary>
		/// Whether the WaveManager starts waves on Awake - defaulted to null since the LevelManager should call this function
		/// </summary>
		public bool startWavesOnAwake;

		/// <summary>
		/// The waves to run in order
		/// </summary>
		[Tooltip("Specify this list in order")]
		public List<Wave> waves = new List<Wave>();

		/// <summary>
		/// The current wave number
		/// </summary>
		public int waveNumber
		{
			get { return m_CurrentIndex + 1; }
		}

		/// <summary>
		/// The total number of waves
		/// </summary>
		public int totalWaves
		{
			get { return waves.Count; }
		}

		/// <summary>
		/// Get the current amount of waves running
		/// </summary>
		public Wave activeWave;

		public float waveProgress
		{
			get
			{
				if (waves == null || waves.Count <= m_CurrentIndex)
				{
					return 0;
				}
				return waves[m_CurrentIndex].progress;
			}
		}

		/// <summary>
		/// Called when a wave begins
		/// </summary>
		public event Action waveChanged;

		/// <summary>
		/// Called when all waves are finished
		/// </summary>
		public event Action spawningCompleted;

		/// <summary>
		/// Starts the waves
		/// </summary>
		public virtual void StartWaves()
		{
			if (waves.Count > 0)
			{
				InitCurrentWave();
			}
			else
			{
				Debug.LogWarning("[LEVEL] No Waves on wave manager. Calling spawningCompleted");
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// Inits the first wave
		/// </summary>
		protected virtual void Awake()
		{
			LevelManager.instance.resetAll += ResetWaves;
			if (startWavesOnAwake)
			{
				StartWaves();
			}
		}

		/// <summary>
		/// Sets up the next wave
		/// </summary>
		protected virtual void NextWave()
		{
			waves[m_CurrentIndex].waveCompleted -= NextWave;
			if (waves.Next(ref m_CurrentIndex))
			{
				InitCurrentWave();
			}
			else
			{
				SafelyCallSpawningCompleted();
			}
		}

		/// <summary>
		/// Initialize the current wave
		/// </summary>
		protected virtual void InitCurrentWave()
		{
			Wave wave = waves[m_CurrentIndex];
			wave.waveNumber = m_CurrentIndex + 1;
			wave.waveCompleted += NextWave;
			activeWave = wave;
			wave.Init();
			if (waveChanged != null)
			{
				waveChanged();
			}
		}

		/// <summary>
		/// Calls spawningCompleted event
		/// </summary>
		protected virtual void SafelyCallSpawningCompleted()
		{
			if (spawningCompleted != null)
			{
				spawningCompleted();
			}
		}

		public void ResetWaves()
		{
			//Generate for loop in waves
            m_CurrentIndex = 0;
			activeWave.spawnInstructions = waves[0].spawnInstructions;
			activeWave.ResetWave();
        }
	}
}