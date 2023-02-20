using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class AccelerometerRotation : MonoBehaviour
{
    private PisonManager _pisonManager;


    void Awake()
    {
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();
    }

    void Update()
    {
        transform.localRotation = new Quaternion(_pisonManager.currentDeviceAccelerometer.x, _pisonManager.currentDeviceAccelerometer.y, _pisonManager.currentDeviceAccelerometer.z, 0);
    }
}
