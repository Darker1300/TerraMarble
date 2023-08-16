using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct WaveData
{
    public string WaveNumber;
    public AnimationCurve smallExploderCurve;
    public float smallExploderInterval;
    public int smallExploderSpawnAmount;
    public AnimationCurve trackerCurve;
    public float trackerInterval;
    public int trackerSpawnAmount;
    


}
public class WaveManager : MonoBehaviour
{
    [SerializeField]
    public List<WaveData> WavesInfo = new List<WaveData>();

    public int TotalEnemiesActive;

    public EnemyWaveController smallExploder;
    public EnemyWaveController trackerExploder;
    public int CurrentWave = 0;
    public int CurrentEnemies;
    public void UpdateWave()
    {
        CurrentWave++;
        UpdateWaveData();
    }
    public void UpdateWaveData()
    {

        
        smallExploder.UpdateEnemywaveCurve(WavesInfo[CurrentWave].smallExploderCurve, WavesInfo[CurrentWave].smallExploderInterval, WavesInfo[CurrentWave].smallExploderSpawnAmount);
        trackerExploder.UpdateEnemywaveCurve(WavesInfo[CurrentWave].trackerCurve, WavesInfo[CurrentWave].trackerInterval, WavesInfo[CurrentWave].smallExploderSpawnAmount);
        





    }
    // Start is called before the first frame update
    void Start()
    {
        UpdateWaveData();
        
    }
    
    // Update is called once per frame
    void Update()
    {

    }
}
