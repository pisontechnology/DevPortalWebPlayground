using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class QuaternionRotation : MonoBehaviour
{
    private PisonEvents _pisonEvents;

    [SerializeField] private GameObject objToRotate;

    #region Pison Functions

    private void OnQuaternion(Quaternion rotation)
    {
        if(objToRotate != null)
        {
            objToRotate.transform.rotation = rotation;
        }
        else
        {
            transform.rotation = rotation;
        }
    }

    #endregion

    #region Unity Functions

    void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

        _pisonEvents.OnQuaternion += OnQuaternion;
    }

    void OnDestroy()
    {
        _pisonEvents.OnQuaternion -= OnQuaternion;
    }

    #endregion
}
