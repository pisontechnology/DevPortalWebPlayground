using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class ShakeDemoManager : MonoBehaviour
{
    private PisonEvents _pisonEvents;
    private PisonManager _pisonManager;

    [SerializeField] private GameObject box;
    private Quaternion boxOGRotation;
    [SerializeField] private Transform shootPoint;

    [SerializeField] private GameObject[] particleList;

    private bool shakeBox;
    private bool calledOtherShake;

    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeSpeed;
    [SerializeField] private float shakeAmount;

    #region Private Functions

    private IEnumerator ShakeTrigger()
    {
        yield return new WaitForSeconds(.5f);

        if (!calledOtherShake)
        {
            shakeBox = true;
            _pisonManager.SendHapticOn();
            StartCoroutine(ShakeSequence());

            yield return new WaitForSeconds(shakeDuration);

            box.transform.rotation = new Quaternion(boxOGRotation.x, boxOGRotation.y, boxOGRotation.z, boxOGRotation.w);

            shakeBox = false;
            _pisonManager.SendHapticOff();
        }
    }

    private IEnumerator ShakeSequence()
    {
        int dir = 1;
        float timer = 0;
        float toggleTime = 0.1f;
        float maxAngle = 45f;

        while (shakeBox)
        {
            timer += Time.deltaTime;
            float zAngle = dir * timer * (maxAngle / toggleTime);
            float xAngle = dir * timer * (maxAngle / toggleTime);
            float yAngle = dir * timer * (maxAngle / toggleTime);
            box.transform.rotation = Quaternion.Euler(xAngle, yAngle, zAngle);
            if(timer >= toggleTime)
            {
                dir = dir * -1;
                timer = 0;
            }
            yield return null;
        }
    }

    private IEnumerator DestroyParticle(float waitTime, GameObject desiredParticle)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(desiredParticle);
    }

    private void SpawnParticle(GameObject desiredParticle, float lifeTime)
    {
        Vector3 pos = shootPoint.position;
        Quaternion rot = desiredParticle.transform.rotation;

        GameObject particle = Instantiate(desiredParticle, pos, rot);
        _pisonManager.SendHapticBursts(500, 1, 200);

        StartCoroutine(DestroyParticle(lifeTime, particle));
    }

    #endregion

    #region Pison Functions

    private void OnShake(string shakeGesture)
    {
        Debug.Log(shakeGesture);
        
        switch (shakeGesture)
        {
            case "SHAKE":
                calledOtherShake = false;
                StartCoroutine(ShakeTrigger());
                break;

            case "SHAKE_N_INDEX":
                calledOtherShake = true;
                SpawnParticle(particleList[0], 8);
                break;

            case "SHAKE_N_THUMB":
                calledOtherShake = true;
                SpawnParticle(particleList[1], 10);
                break;

            case "SHAKE_N_HAND":
                calledOtherShake = true;
                break;
        }
    }

    #endregion

    #region Unity Function

    private void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();

        _pisonEvents.OnShake += OnShake;

        boxOGRotation = new Quaternion(box.transform.rotation.x, box.transform.rotation.y, box.transform.rotation.z, box.transform.rotation.w);
    }

    private void Update()
    {

    }

    private void OnDestroy()
    {
        _pisonEvents.OnExtension -= OnShake;
    }

    #endregion

}
