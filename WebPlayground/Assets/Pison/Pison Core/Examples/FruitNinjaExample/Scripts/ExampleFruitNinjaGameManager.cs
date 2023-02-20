using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 
/// Handels Start, End, and Starting spawning sequence.
/// Updates UI, from tutorial, Health updates, Score updates, and End panel
/// Handels Player health and stoping all needed sequences for when player dies, or restarts game
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleFruitNinjaGameManager : MonoBehaviour
{
    public static ExampleFruitNinjaGameManager instance;

    private ExampleSpawnManager spawnManager;

    public bool isPlayerAlive = true;

    [Header("Player Variables:")]
    [SerializeField] private int curHealth;
    [SerializeField] private int playerScore;
    [HideInInspector] public int fruitsSlashed;

    [HideInInspector] public bool gameStart;

    [HideInInspector] public float totalTime;
    [HideInInspector] public float startTime;

    [Header("UI Variables: ")]
    [SerializeField] private GameObject howToPanel;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text endScoreText;
    [SerializeField] private Text endTimeText;

    [SerializeField] private Image[] heartList;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    #region Public Functions

    // Called whenever you want to do damage to the player
    public void TakeDamage(in int damage)
    {
        curHealth -= damage;

        curHealth = Mathf.Clamp(curHealth, 0, heartList.Length);

        // Updates UI hearts
        UpdateHearts();

        // Ends game if player is dead
        if(curHealth <= 0)
        {
            EndGame();
        }
    }

    // displays and resets player in game score
    public void UpdateScore(in int score)
    {
        if(score != -100)
        {
            playerScore += score;
            scoreText.text = playerScore.ToString("F0");
        }
        else
        {
            playerScore = 0;
            scoreText.text = playerScore.ToString("F0");
        }
    }

    // Not currently used, but implemetned incase a system to add health back to player is wanted to be implemented
    public void AddHealth(in int health)
    {
        curHealth += health;

        curHealth = Mathf.Clamp(curHealth, 0, heartList.Length);

        UpdateHearts();
    }

    #endregion


    #region UI Manager

    // Changes the amount of heart sprites on screen
    private void UpdateHearts()
    {
        for(int i = 0; i < heartList.Length; ++i)
        {
            if(i < curHealth)
            {
                heartList[i].sprite = fullHeart;
            }
            else
            {
                heartList[i].sprite = emptyHeart;
            }
        }
    }

    #endregion


    #region Game Sequence Functions

    #region Public Game Sequence Functions

    // Set up all necisary systems to start game, along with reset all systems incase a restart is called
    public void StartGame()
    {
        // Change to ingame UI
        howToPanel.SetActive(false);
        scorePanel.SetActive(true);
        startPanel.SetActive(false);
        endPanel.SetActive(false);

        // Start Spawning Sequence here
        spawnManager.StartSpawning();
        
        curHealth = heartList.Length;
        startTime = Time.deltaTime;
        Time.timeScale = 0.8f;
        totalTime = 0;
        fruitsSlashed = 0;

        gameStart = true;
        isPlayerAlive = true;

        ExampleFruit[] aliveFruit = GameObject.FindObjectsOfType<ExampleFruit>();

        if (aliveFruit.Length > 0)
        {
            for (int i = 0; i < aliveFruit.Length; ++i)
            {
                Destroy(aliveFruit[i].gameObject);
            }
        }

        UpdateScore(-100);
        UpdateHearts();
    }

    #endregion

    // Will freeze all mechanics and display end panel for player
    private void EndGame()
    {
        Time.timeScale = 0;
        Debug.Log("Player Died");
        isPlayerAlive = false;
        gameStart = false;

        endPanel.SetActive(true);

        endScoreText.text = playerScore.ToString("F0");

        totalTime -= startTime;

        float minutes = Mathf.FloorToInt(totalTime / 60);
        float seconds = Mathf.FloorToInt(totalTime % 60);

        endTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // inicalazation process of basic systems
    private void StartSetup()
    {
        instance = this;

        curHealth = heartList.Length;

        spawnManager = GetComponent<ExampleSpawnManager>();

        Time.timeScale = 0.8f;
    }

    #endregion


    #region Unity Function

    private void Awake()
    {
        StartSetup();
    }

    private void Update()
    {
        if (gameStart)
        {
            totalTime += Time.deltaTime;
        }
    }

    #endregion
}
