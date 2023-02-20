using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class EulerAngleRotation : MonoBehaviour
{
    private PisonManager _pisonManager;


    private void Awake()
    {
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();
    }

    private void Update()
    {
        transform.localEulerAngles = _pisonManager.currentDeviceEulerAngles;
    }
}
