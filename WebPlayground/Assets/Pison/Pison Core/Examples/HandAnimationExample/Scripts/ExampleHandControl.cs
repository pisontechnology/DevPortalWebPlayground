using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;
using PisonCore;
using Quaternion = UnityEngine.Quaternion;

/// <summary>
/// 
/// Handles control over hand model in example, such as
/// Rotation off of Quaternion functions
/// Hand animation off of gesture function
/// and some resetting and zeroing functionality
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleHandControl : MonoBehaviour
{
    private PisonManager pisonManager;
    private PisonEvents pisonEvents;

    private Animator currentAnimator;

    public bool useDebounce;
    public bool lockRoation;
    private bool debounce;
    private bool resetRotation = false;

    [SerializeField] private Vector4 zeroPoint;
    private bool recallibrateZeroPoint;

    private bool inputDelay;
    [SerializeField] private float inputDelayAmount;
    private float currentTime;

    [SerializeField] private GameObject rightHandDebounce;
    [SerializeField] private GameObject rightHandNoDebounce;

    #region Public Functions

    public void ToggleDebounce()
    {
        // Switching between using debounce and not
        useDebounce = !useDebounce;

        if (useDebounce)
        {
            rightHandDebounce.SetActive(true);
            rightHandNoDebounce.SetActive(false);
            currentAnimator = rightHandDebounce.GetComponent<Animator>();
        }
        else
        {
            rightHandDebounce.SetActive(false);
            rightHandNoDebounce.SetActive(true);
            currentAnimator = rightHandNoDebounce.GetComponent<Animator>();
        }
    }

    // Handles triggers for locking/reseting hand rotation
    public void ToggleRotation()
    {
        lockRoation = !lockRoation;
        resetRotation = true;
    }

    #endregion

    #region Pison API

    // Basic debounced based method of reading gestures
    private void ExtensionsDebounce(string gesture)
    {
        if (gesture == "REST")
        {
            currentAnimator.SetBool("Index", false);
            currentAnimator.SetBool("Thumb", false);
            currentAnimator.SetBool("Hand", false);
            debounce = false;
        }

        if (!debounce)
        {
            switch (gesture)
            {
                case "INDEX":
                    currentAnimator.SetBool("Index", true);
                    currentAnimator.SetBool("Thumb", false);
                    currentAnimator.SetBool("Hand", false);
                    debounce = true;
                    break;
                
                case "THUMB":
                    currentAnimator.SetBool("Index", false);
                    currentAnimator.SetBool("Thumb", true);
                    currentAnimator.SetBool("Hand", false);
                    debounce = true;
                    break;
                
                case "HAND":
                    currentAnimator.SetBool("Index", false);
                    currentAnimator.SetBool("Thumb", false);
                    currentAnimator.SetBool("Hand", true);
                    debounce = true;
                    break;
            }
            
        }
    }

    private IEnumerator ExtensionsNoDebounce(string gesture)
    {
        if (inputDelay)
        {
            inputDelay = false;
            
            // Decides which gesture is called
            // yield return new WaitForSeconds() called to make animations smooth on model
            switch (gesture)
            {
                case "INDEX":
                    currentAnimator.SetBool("Index", true);
                    yield return new WaitForSeconds(0.3f);
                    currentAnimator.SetBool("Thumb", false);
                    currentAnimator.SetBool("Hand", false);
                    break;
                    
                case "THUMB":
                    currentAnimator.SetBool("Thumb", true);
                    yield return new WaitForSeconds(0.3f);
                    currentAnimator.SetBool("Index", false);
                    currentAnimator.SetBool("Hand", false);
                    break;
                    
                case "HAND":
                    currentAnimator.SetBool("Hand", true);
                    yield return new WaitForSeconds(0.3f);
                    currentAnimator.SetBool("Index", false);
                    currentAnimator.SetBool("Thumb", false);
                    break;
                
                case "REST":
                    currentAnimator.SetBool("Index", false);
                    currentAnimator.SetBool("Thumb", false);
                    currentAnimator.SetBool("Hand", false);
                    break;
            }
        }
    }
    
    
    private void OnExtension(string gesture)
    {
        // Mini Delay to smooth transitions between animations
        if (inputDelay)
        {
            Debug.Log(gesture);
            
            if (useDebounce)
            {
                ExtensionsDebounce(gesture);
            }
            else
            {
                StartCoroutine(ExtensionsNoDebounce(gesture));
            }
        }
    }

    private void OnQuaterion(Quaternion newRot)
    {
        if (resetRotation)
        {
            zeroPoint = new Vector4(newRot.x, newRot.y, newRot.z, newRot.w);
            resetRotation = false;
        }
        
        if (!lockRoation)
        {
            //Debug.Log("rotate");
            
            // I don't want to do this method but my hands a tied, Unity Build wont accept AnimationController variables and calls errors
            // new method switches between two different GameObjects that have the two different controllers needed for animations

            Vector4 zeroVector4 = new Vector4(0, 0, 0, 0);

            if (zeroPoint == zeroVector4)
            {
                if(rightHandDebounce != null)
                    rightHandDebounce.transform.rotation = new Quaternion(-newRot.y, -newRot.z, newRot.x, newRot.w);
                
                if(rightHandNoDebounce != null)
                    rightHandNoDebounce.transform.rotation = new Quaternion(-newRot.y, -newRot.z, newRot.x, newRot.w);
            }
            else
            {
                // Get differance between last unlocked rotation point and current new quaternion data
                Quaternion zeroQuatern = new Quaternion(zeroPoint.x, zeroPoint.y, zeroPoint.z, zeroPoint.w);
                Quaternion diff = Quaternion.Inverse(zeroQuatern) * newRot;

                if(rightHandDebounce != null)
                    rightHandDebounce.transform.rotation = new Quaternion(-diff.y, -diff.z, diff.x, diff.w);
                
                if(rightHandNoDebounce != null)
                    rightHandNoDebounce.transform.rotation = new Quaternion(-diff.y, -diff.z, diff.x, diff.w);
            }
        }
    }

    private void OnDeviceConnection(ConnectedDevice device, bool connected)
    {
        // On connection zero device
        if (connected)
        {
            zeroPoint = new Vector4(pisonManager.currentDeviceQuaternion.x,
                                    pisonManager.currentDeviceQuaternion.y,
                                    pisonManager.currentDeviceQuaternion.z,
                                    pisonManager.currentDeviceQuaternion.w);
        }
    }

    #endregion
    
    #region Unity Functions

    private void Awake()
    {
        // Basic set up for Pison UnitySDK
        pisonManager = GameObject.FindObjectOfType<PisonManager>();
        pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        
        pisonEvents.OnExtension += OnExtension;
        pisonEvents.OnQuaternion += OnQuaterion;
        pisonEvents.OnDeviceConnection += OnDeviceConnection;

        currentTime = inputDelayAmount;

        recallibrateZeroPoint = false;
        
        // Set up proper GameObject based off of starting variables
        if (useDebounce)
        {
            rightHandDebounce.SetActive(true);
            rightHandNoDebounce.SetActive(false);
            currentAnimator = rightHandDebounce.GetComponent<Animator>();
        }
        else
        {
            rightHandDebounce.SetActive(false);
            rightHandNoDebounce.SetActive(true);
            currentAnimator = rightHandNoDebounce.GetComponent<Animator>();
        }
    }

    private void Update()
    {
        // Delay calculator
        currentTime += Time.deltaTime;

        if (currentTime > inputDelayAmount)
        {
            currentTime = 0;
            inputDelay = true;
        }
    }

    #endregion
    
}
