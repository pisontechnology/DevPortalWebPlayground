using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Handels spawning of individual fruits from ExampleSpawnManager
/// Such as direction, force, delay, amount
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleSpawnPoint : MonoBehaviour
{
    [SerializeField] private GameObject fruitPrefab;

    [SerializeField] private float minThrowForce;
    [SerializeField] private float maxThrowForce;

    [Tooltip("Positive numbers only")]
    [SerializeField] private float maxLeftAngle;
    [Tooltip("Negative numbers only")]
    [SerializeField] private float maxRightAngle;

    [HideInInspector] public float spawnRate;

    Vector3 ogRotation;

    #region Spawn Functions

    public IEnumerator SpawnFruit(float fruitAmount, float delay, float throwForce, float isSameAngle)
    {
        if (fruitAmount >= 2)
        {
            GetRandomAngle();

            // Spawn cycled fruit
            for (int i = 0; i < fruitAmount; ++i)
            {
                if ((int)isSameAngle == 0)
                {
                    // Spawn multiple Fruit of same angel
                    SpawnIndividualFruit(throwForce);
                }
                else
                {
                    // Spawn multipleFruit of different angels
                    GetRandomAngle();
                    SpawnIndividualFruit(throwForce);
                }

                yield return new WaitForSeconds(delay);
            }
        }
        else
        {
            // Spawn one Fruit
            GetRandomAngle();
            SpawnIndividualFruit(throwForce);

            yield return null;

        }
    }

    // How actual spawning works
    private void SpawnIndividualFruit(float throwForce)
    {
        GameObject fruit = Instantiate(fruitPrefab, transform.position, transform.rotation);

        if(throwForce == 0)
        {
            fruit.GetComponent<Rigidbody2D>().AddForce(transform.up * throwForce, ForceMode2D.Impulse);
        }
        else
        {
            fruit.GetComponent<Rigidbody2D>().AddForce(transform.up * Random.Range(minThrowForce, maxThrowForce), ForceMode2D.Impulse);
        }
    }

    // Gets random angle controled by certain spawn point to shoot fruit out from
    private void GetRandomAngle()
    {
        transform.localEulerAngles = ogRotation;

        transform.localEulerAngles = new Vector3(0, 0, Random.Range(maxLeftAngle, maxRightAngle));
    }

    #endregion


    #region Unity Functions

    private void Awake()
    {
        ogRotation = transform.localEulerAngles;
    }

    #endregion
}
