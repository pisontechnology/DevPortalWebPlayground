using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Handels damaging the player when Fruit falls off screen
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleDamageManager : MonoBehaviour
{

    #region Unity Functions

    // if fruit goes off screen and hits trigger will damage the player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name.StartsWith("Fruit"))
        {
            ExampleFruitNinjaGameManager.instance.TakeDamage(collision.GetComponent<ExampleFruit>().totalDamage);
            Destroy(collision.gameObject);
        }
    }

    #endregion
}
