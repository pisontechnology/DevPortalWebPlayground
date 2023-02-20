using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Handels the types spawn rates, spawn patterns and which spawn point will spawn fruit
/// Layouts spawn points so they always allign with move points no matter the screen size
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleSpawnManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints = new List<GameObject>(new GameObject[4]);
    [Header("Possible Spawn Formations: ")]
    [Tooltip("X = Amount of Fruit, Y = spawn Delay, Z = Throw Force (0 = Random), W = (ONLY!!! 0 or 1) Should be same angle")]
    [SerializeField] Vector4[] spawnFormations;
    int spawnPointToUse;

    [SerializeField] private float spawnRate;

    [HideInInspector] public bool shouldSpawn;
    private bool onlySpawnOneSequence;
    private int increaseStage = 0;


    #region Public Functions

    // Will start spawning sequence when game starts
    public void StartSpawning()
    {
        shouldSpawn = true;
        StartCoroutine(SpawningSequence());
    }

    #endregion


    #region Spawn Functions

    // Tells which spawner to spawn amount of fruit, direction, force, and delays
    // Gets info from premade system called 'spawnFormations' (See in inspector)
    private IEnumerator SpawningSequence()
    {
        while (ExampleFruitNinjaGameManager.instance.isPlayerAlive && shouldSpawn)
        {
            // Delay
            yield return new WaitForSeconds(spawnRate);

            // Makes sure there are no fruit still on screen before spawning more fruit
            if (onlySpawnOneSequence)
            {
                ExampleFruit[] aliveFruit = GameObject.FindObjectsOfType<ExampleFruit>();

                while (aliveFruit.Length > 0)
                {
                    //Debug.Log("Delay Spawn");

                    yield return new WaitForSeconds(0.2f);

                    aliveFruit = GameObject.FindObjectsOfType<ExampleFruit>();
                }
            }

            // Spawn Fruit
            int select = Random.Range(0, spawnFormations.Length);

            spawnPointToUse = Random.Range(0, spawnPoints.Count);

            StartCoroutine(spawnPoints[spawnPointToUse].GetComponent<ExampleSpawnPoint>().SpawnFruit(spawnFormations[select].x,
                                                                                      spawnFormations[select].y,
                                                                                      spawnFormations[select].z,
                                                                                      spawnFormations[select].w));

            // Update spawn rate if need be
            ControlSpawnRate();
        }
    }

    // As the game progresses will incramentally increase the difficulty for the player, until max difficulty where it becomes an endurances test
    private void ControlSpawnRate()
    {
        float curTime = ExampleFruitNinjaGameManager.instance.totalTime - ExampleFruitNinjaGameManager.instance.startTime;

        // Check if a minute passed
        if (curTime >= 30 && increaseStage == 0)
        {
            increaseStage++;
            //Debug.Log("30 seconds");
            spawnRate -= 1;
        }
        else if (curTime >= 50 && increaseStage == 1)
        {
            increaseStage++;
            //Debug.Log("50 seconds");
            spawnRate -= 1;
        }
        else if (curTime >= 75 && increaseStage == 2)
        {
            increaseStage++;
            //Debug.Log("75 seconds");
            spawnRate -= 1;
        }
        else if (curTime >= 90 && increaseStage == 3)
        {
            increaseStage++;
            //Debug.Log("90 seconds");
            spawnRate -= 0.5f;
            onlySpawnOneSequence = false;
        }
    }

    #endregion


    #region Unity Functions

    /// <summary>
    /// Gets screen ratio then breaks it into 1/4ths along the bottom of the screen, this will automatically match with move positions.
    /// Then saves those positions and moves 4 given spawn points to their own designated location for spawning fruit later
    /// </summary>
    private void Awake()
    {
        // Setting rows and collums to work on any screen size/shape
        float quaterWidthScreen = Screen.width / 4f;

        Vector3 pos1 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen, 1.5f, 10));
        Vector3 pos2 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 2, 1.5f, 10));
        Vector3 pos3 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 3, 1.5f, 10));
        Vector3 pos4 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 4, 1.5f, 10));

        pos1.x = pos1.x - 2f;
        pos2.x = pos2.x - 2f;
        pos3.x = pos3.x - 2f;
        pos4.x = pos4.x - 2;

        // Setting Spawn Points to new locations
        spawnPoints[0].transform.position = pos1;
        spawnPoints[1].transform.position = pos2;
        spawnPoints[2].transform.position = pos3;
        spawnPoints[3].transform.position = pos4;

        // Makes sure spawning doest start on
        shouldSpawn = false;

        // Stops an overflow of fruit from being on screen
        onlySpawnOneSequence = true;
    }

    #endregion
}
