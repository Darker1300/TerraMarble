using System.Collections;
using UnityEngine;

public class EnemyWaveController : MonoBehaviour
{
    //public GameObject enemyPrefab;
    public AnimationCurve spawnCurve;
    public float spawnInterval = 1.0f;
    public int numberOfSpawn = 3;
    //public int MaxEnemyAmount;
    private int SpawnCount = 0;
    private float elapsedTime = 0.0f;
    public SpawnRandomUnitCirclePos spawnRandomCirclePos;
    public WaveManager waveManager;
    private void Start()
    {
        waveManager = GetComponentInParent<WaveManager>();
    }
    private void Update()
    {
        if (SpawnCount < numberOfSpawn)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= spawnInterval)
            {
                elapsedTime = 0.0f;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        float spawnProgress = Mathf.Clamp01(SpawnCount / (float)(numberOfSpawn - 1));
        float curveValue = spawnCurve.Evaluate(spawnProgress);
        int enemiesToSpawn = Mathf.RoundToInt(curveValue); // Adjust the multiplier as needed
        
        for (int i = 0; i < enemiesToSpawn; i++)
        {
            spawnRandomCirclePos.SpawnController();

            waveManager.TotalEnemiesActive += spawnRandomCirclePos.amount;

            //Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }

        SpawnCount++;

        if (SpawnCount >= numberOfSpawn)
        {
            Debug.Log("Wave spawning complete.");
        }


    }
    public void UpdateEnemywaveCurve(AnimationCurve animationCurve,float spawnIntervals,int numberOfSpawns)
    {

        spawnCurve = animationCurve;
        spawnInterval = spawnIntervals;
        numberOfSpawn = numberOfSpawns;


    }
}