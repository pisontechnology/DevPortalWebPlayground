using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class GyroRoation : MonoBehaviour
{
    private PisonManager _pisonManager;


    void Awake()
    {
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();
    }


    void Update()
    {
        transform.localRotation = new Quaternion(_pisonManager.currentDeviceGyro.x, _pisonManager.currentDeviceGyro.y, _pisonManager.currentDeviceGyro.z, 0); ;
    }
}
