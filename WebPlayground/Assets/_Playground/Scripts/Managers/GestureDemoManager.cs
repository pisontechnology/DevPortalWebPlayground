using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class GestureDemoManager : MonoBehaviour
{
    private PisonEvents _pisonEvents;

    [SerializeField] private Animator currentAnimator;
    
    #region Pison Functions

    private void OnExtension(string gesture)
    {
        Debug.Log(gesture);

        switch (gesture)
        {
            case "INDEX":
                currentAnimator.SetBool("Index", true);
                currentAnimator.SetBool("Thumb", false);
                currentAnimator.SetBool("Hand", false);
                break;

            case "THUMB":
                currentAnimator.SetBool("Index", false);
                currentAnimator.SetBool("Thumb", true);
                currentAnimator.SetBool("Hand", false);
                break;

            case "HAND":
                currentAnimator.SetBool("Index", false);
                currentAnimator.SetBool("Thumb", false);
                currentAnimator.SetBool("Hand", true);
                break;

            default:
                currentAnimator.SetBool("Index", false);
                currentAnimator.SetBool("Thumb", false);
                currentAnimator.SetBool("Hand", false);
                break;
        }
    }

    #endregion

    #region Unity Function

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
