using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;
using UnityEngine.UI;

public class DeviceConnectedUI : MonoBehaviour
{
    private PisonManager _pisonManager;

    [SerializeField] private Image connectedImg;
    [SerializeField] private Image disconnectedImg;

    void Awake()
    {
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();
    }

    void Update()
    {
        if (_pisonManager.deviceConnected)
        {
            connectedImg.enabled = true;
            disconnectedImg.enabled = false;
        }
        else
        {
            connectedImg.enabled = false;
            disconnectedImg.enabled = true;
        }
    }
}
