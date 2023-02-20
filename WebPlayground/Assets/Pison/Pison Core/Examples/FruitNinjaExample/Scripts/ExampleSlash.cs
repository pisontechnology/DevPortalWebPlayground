using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 
/// Spawns and manages slash/particle effects of the Slash
/// Also manages destroying Fruit and updating ExampleFruitNinjaGameManager with score
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleSlash : MonoBehaviour
{
    private bool slashEnd = false;

    [SerializeField] private GameObject fruitDeathParticle;

    private int aliveParticles;

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private GameObject slashEffect;
    private GameObject slashObject;
    [SerializeField] private float slashSpeed;


    // Controls fruit death particle effects when touching slash spawn and deletion
    private IEnumerator DestroyParticle(ParticleSystem particle)
    {
        yield return new WaitForSeconds(particle.startLifetime);

        Destroy(particle.gameObject);
        aliveParticles--;

        if(aliveParticles <= 0 && slashEnd)
        {
            if (slashObject != null)
                Destroy(slashObject);

            Destroy(gameObject);
        }
    }


    #region Public Functions

    // Call to destroy desired slash
    public void SlashEnd(bool slashState)
    {
        if (aliveParticles <= 0)
        {
            Destroy(slashObject);
            Destroy(gameObject);
        }

        slashEnd = slashState;
    }

    #endregion


    #region Unity Functions
    
    private void Start()
    {
        slashObject = Instantiate(slashEffect, pointA.position, pointA.rotation);
    }

    private void Update()
    {
        // Move slash effect across screen
        float step = slashSpeed * Time.deltaTime;
        slashObject.transform.position = Vector3.MoveTowards(slashObject.transform.position, pointB.position, step);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Updates score and spawns particles when hit fruit
        if (collision.name.StartsWith("Fruit"))
        {
            ExampleFruitNinjaGameManager.instance.UpdateScore(collision.GetComponent<ExampleFruit>().totalScore);
            Destroy(collision.gameObject);
            ExampleFruitNinjaGameManager.instance.fruitsSlashed++;

            GameObject deathParticle = Instantiate(fruitDeathParticle, collision.transform.position, collision.transform.rotation);
            StartCoroutine(DestroyParticle(deathParticle.GetComponent<ParticleSystem>()));
            aliveParticles++;
        }
    }

    #endregion
}
