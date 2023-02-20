using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class GestureClick : MonoBehaviour
{
    public enum GESTURES
    {
        HAND,
        THUMB,
        INDEX
    }
    
    private PisonEvents _pisonEvents;

    [SerializeField] private GESTURES[] possibleGesture;
    [SerializeField] private float clickTime = 0.4f;
    
    [HideInInspector] public bool handClick;
    [HideInInspector] public bool thumbClick;
    [HideInInspector] public bool indexClick;

    private bool handActive;
    private bool thumbActive;
    private bool indexActive;

    #region Pison Functions

    private void OnExtension(string gesture)
    {
        for (int i = 0; i < possibleGesture.Length; ++i)
        {
            if (gesture == possibleGesture[i].ToString())
                StartCoroutine(ClickSequence(possibleGesture[i]));
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
                handClick = false;
                thumbClick = false;
                indexClick = false;
                break;
        }
    }

    private IEnumerator ClickSequence(GESTURES desiredGestures)
    {
        yield return new WaitForSeconds(clickTime);

        switch (desiredGestures)
        {
            case GESTURES.HAND:
                if (handActive)
                {
                    handClick = true;
                    thumbClick = false;
                    indexClick = false;
                }
                break;

            case GESTURES.THUMB:
                if (thumbActive)
                {
                    handClick = false;
                    thumbClick = true;
                    indexClick = false;
                }
                break;

            case GESTURES.INDEX:
                if (indexActive)
                {
                    handClick = false;
                    thumbClick = false;
                    indexClick = true;
                }
                break;
        }

        yield return new WaitForSeconds(.5f);

        handClick = false;
        thumbClick = false;
        indexClick = false;
    }

    #endregion
    
    #region Unity Functions

    private void Awake()
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
