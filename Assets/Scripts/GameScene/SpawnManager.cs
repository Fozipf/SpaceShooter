using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;

    private bool _stopSpawning = false;

    private int _currentWave = 1;
    private int _totalWaves = 10;
    public static int _enemiesKilled = 0;

    private float _startTime;
    private float _waveTime = 20f;
    private float _initialSpawnPause = 5f;
    private int[] enemiesPerWave = { 10,15,20,25,35,45,52,64,80,100};
    private float[] spawnPausePerWave = { 5, 4.6f, 4.3f, 3.9f, 3.5f, 3.1f, 2.8f, 2.6f, 2.3f, 2f };

    [SerializeField]
    private UIManager _uiManager;

    private void Update()
    {
        _uiManager.UpdateWaveCountdown((_waveTime*_currentWave+_startTime)-Time.time);
    }


    public void StartSpawning()
    {
        _startTime = Time.time;
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator SpawnEnemyRoutine()
    {

        //yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            float spawnPause = _initialSpawnPause - 0.3f * (_currentWave - 1);
            Debug.Log("Spawn Pause: " + spawnPause);
            yield return new WaitForSeconds(spawnPause);
            
            Vector3 _spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, _spawnPosition, Quaternion.identity);
            
            newEnemy.transform.parent = _enemyContainer.transform;
            
            
            if(Time.time >= _startTime + _waveTime*_currentWave)
            {
                yield return NextWave();
            }
        }
    }

    IEnumerator NextWave()
    {
        _enemiesKilled = 0;
        _currentWave++;
        Debug.Log("Starting Wave " + _currentWave);
        yield return new WaitForSeconds(3f);
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (!_stopSpawning)
        {
            int randomPowerupID = Random.Range(0, _powerups.Length);

            Vector3 _spawnPosition = new Vector3(Random.Range(-8f, 8f), 7, 0);

            if (randomPowerupID == 5)
            {
                int random = Random.Range(0, 4);

                if (random == 0)
                {
                    Instantiate(_powerups[randomPowerupID], _spawnPosition, Quaternion.identity);
                }
            }
            else
            {
                Instantiate(_powerups[randomPowerupID], _spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(3, 8));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
