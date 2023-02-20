using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;
using UnityEngine.XR;

public class GestureHold : MonoBehaviour
{
    public enum GESTURES
    {
        HAND,
        THUMB,
        INDEX
    }

    private PisonEvents _pisonEvents;

    [SerializeField] private GESTURES[] possibleGesture;
    [SerializeField] private float holdTime = 0.4f;
    
    [HideInInspector] public bool handHold;
    [HideInInspector] public bool thumbHold;
    [HideInInspector] public bool indexHold;

    private bool handActive;
    private bool thumbActive;
    private bool indexActive;

    #region Pison Functions

    private void OnExtension(string gesture)
    {
        for (int i = 0; i < possibleGesture.Length; ++i)
        {
            if (gesture == possibleGesture[i].ToString())
                StartCoroutine(HoldSequence(possibleGesture[i]));
        }

        switch (gesture)
        {
            case "HAND":
                handActive = true;
                thumbActive = false;
                indexActive = false;
                break;
            
            case "THUMB":
                handActive = false;
                thumbActive = true;
                indexActive = false;
                break;
            
            case "INDEX":
                handActive = false;
                thumbActive = false;
                indexActive = true;
                break;
            
            default:
                handHold = false;
                thumbHold = false;
                indexHold = false;
                break;
        }
    }

    private IEnumerator HoldSequence(GESTURES desiredGestures)
    {
        yield return new WaitForSeconds(holdTime);

        switch (desiredGestures)
        {
            case GESTURES.HAND:
                if (handActive)
                {
                    handHold = true;
                    thumbHold = false;
                    indexHold = false;
                }
                break;
            
            case GESTURES.THUMB:
                if (thumbActive)
                {
                    handHold = false;
                    thumbHold = true;
                    indexHold = false;
                }
                break;
            
            case GESTURES.INDEX:
                if (indexActive)
                {
                    handHold = false;
                    thumbHold = false;
                    indexHold = true;
                }
                break;
        }
    }

    #endregion
    
    #region Unity Functions
    
    void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

        _pisonEvents.OnExtension += OnExtension;
    }

    private void OnDestroy()
    {
        _pisonEvents.OnExtension -= OnExtension;
    }

    #endregion
}
