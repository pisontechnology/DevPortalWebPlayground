using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Holds needed data for Fruit
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleFruit : MonoBehaviour
{
    public int totalDamage;
    [HideInInspector] public int totalScore;

    [SerializeField] private int minTotalScore;
    [SerializeField] private int maxTotalScore;

    #region Unity Functions

    private void Awake()
    {
        // Randomly choose this fruits score amount
        totalScore = Random.Range(minTotalScore, maxTotalScore);
    }

    #endregion
}
