using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;
using UnityEngine.UI;
using TMPro;

public class RawDataScript : MonoBehaviour
{
    private PisonManager _pisonManager;
    private PisonEvents _pisonEvents;

    [SerializeField] private TextMeshProUGUI gestureTxt;
    [SerializeField] private TextMeshProUGUI swipeTxt;
    [SerializeField] private TextMeshProUGUI shakeTxt;
    [SerializeField] private TextMeshProUGUI eulerTxt;
    [SerializeField] private TextMeshProUGUI quaternionTxt;
    [SerializeField] private TextMeshProUGUI gyroTxt;
    [SerializeField] private TextMeshProUGUI accelTxt;

    [SerializeField] private Color activeColor;
    [SerializeField] private Color deactiveColor;

    [SerializeField] private float activeTime;

    private bool activated;
    private bool reactivated;

    #region Pison Functions

    private IEnumerator SwipeActivateText()
    {
        if (activated)
            reactivated = true;
        
        if(!activated)
            activated = true;

        swipeTxt.color = activeColor;
        
        yield return new WaitForSeconds(activeTime);

        if (reactivated)
        {
            reactivated = false;
        }
        else
        {
            swipeTxt.color = deactiveColor;
            activated = false;
        }

    }

    private IEnumerator ShakeActivateText()
    {
        if (activated)
            reactivated = true;

        if (!activated)
            activated = true;

        shakeTxt.color = activeColor;

        yield return new WaitForSeconds(activeTime);

        if (reactivated)
        {
            reactivated = false;
        }
        else
        {
            shakeTxt.color = deactiveColor;
            activated = false;
        }
    }

    private void OnExtension(string gesture)
    {
        if(gesture == "INDEX" ||
           gesture == "HAND" ||
           gesture == "THUMB" ||
           gesture == "NAC" ||
           gesture == "NULL" ||
           gesture == "INACTIVE" ||
           gesture == "REST"
           )
        {
            gestureTxt.text = gesture;
        }
        else
        {
            swipeTxt.text = gesture;
            StartCoroutine(SwipeActivateText());
        }
    }

    private void OnShake(string shake)
    {
        shakeTxt.text = shake;
        StartCoroutine(ShakeActivateText());
    }

    #endregion


    #region Unity Functions

    private void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        _pisonManager = GameObject.FindObjectOfType<PisonManager>();

        _pisonEvents.OnExtension += OnExtension;
        _pisonEvents.OnShake += OnShake;
    }

    private void Update()
    {
        eulerTxt.text = "Euler Angle: " + "X: " + _pisonManager.currentDeviceEulerAngles.x.ToString("F2") + 
                                        " Y: " + _pisonManager.currentDeviceEulerAngles.y.ToString("F2") + 
                                        " Z: " + _pisonManager.currentDeviceEulerAngles.z.ToString("F2");

        quaternionTxt.text = "Quaternion: " + "X: " + _pisonManager.currentDeviceQuaternion.x.ToString("F2") +
                                " Y: " + _pisonManager.currentDeviceQuaternion.y.ToString("F2") +
                                " Z: " + _pisonManager.currentDeviceQuaternion.z.ToString("F2") +
                                " W: " + _pisonManager.currentDeviceQuaternion.w.ToString("F2");

        gyroTxt.text = "Gyroscope: " + "X: " + _pisonManager.currentDeviceGyro.x.ToString("F2") +
                                " Y: " + _pisonManager.currentDeviceGyro.y.ToString("F2") +
                                " Z: " + _pisonManager.currentDeviceGyro.z.ToString("F2");

        accelTxt.text = "accelerometer: " + "X: " + _pisonManager.currentDeviceAccelerometer.x.ToString("F2") +
                                " Y: " + _pisonManager.currentDeviceAccelerometer.y.ToString("F2") +
                                " Z: " + _pisonManager.currentDeviceAccelerometer.z.ToString("F2");
    }

    #endregion
}
