using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.pison;
using com.pison.examples.swipe;

public class ChooseGestureScript : MonoBehaviour
{
    private PisonEvents _pisonEvents;

    [SerializeField] private Image handFillBar;
    [SerializeField] private Image thumbFillBar;
    [SerializeField] private Image indexFillBar;

    [SerializeField] private float fillSpeed;

    private bool indexed;
    private bool thumbed;
    private bool handed;

    [SerializeField] private GameObject changeButton;
    [SerializeField] private GameObject choosePanel;

    [SerializeField] private ExampleSwipePlayerController swipePlayerController;


    #region Public Functions

    public void ChangeGesture()
    {
        changeButton.SetActive(false);
        choosePanel.SetActive(true);

        indexFillBar.fillAmount = 0;
        handFillBar.fillAmount = 0;
        thumbFillBar.fillAmount = 0;

        swipePlayerController.indexChosen = false;
        swipePlayerController.thumbChosen = false;
        swipePlayerController.handChosen = false;
    }

    #endregion


    #region Pison Functions

    private void OnExtension(string gesture)
    {
        switch (gesture)
        {
            case "INDEX":
                indexed = true;
                thumbed = false;
                handed = false;
                break;
            case "THUMB":
                indexed = false;
                thumbed = true;
                handed = false;
                break;
            case "HAND":
                indexed = false;
                thumbed = false;
                handed = true;
                break;
            default:
                indexed = false;
                thumbed = false;
                handed = false;
                break;
        }
    }

    #endregion


    #region Unity Functions

    private void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

        _pisonEvents.OnExtension += OnExtension;

        handFillBar.fillAmount = 0;
        thumbFillBar.fillAmount = 0;
        indexFillBar.fillAmount = 0;

        changeButton.SetActive(false);
    }

    private void Update()
    {
        if (indexed)
        {
            indexFillBar.fillAmount += Time.deltaTime * fillSpeed;

            if(indexFillBar.fillAmount == 1)
            {
                swipePlayerController.indexChosen = true;
                changeButton.SetActive(true);
                choosePanel.SetActive(false);
            }
        }
        else if(indexFillBar.fillAmount != 0)
        {
            indexFillBar.fillAmount -= Time.deltaTime * fillSpeed;
        }


        if (thumbed)
        {
            thumbFillBar.fillAmount += Time.deltaTime * fillSpeed;

            if(thumbFillBar.fillAmount == 1)
            {
                swipePlayerController.thumbChosen = true;
                changeButton.SetActive(true);
                choosePanel.SetActive(false);
            }
        }
        else if (thumbFillBar.fillAmount != 0)
        {
            thumbFillBar.fillAmount -= Time.deltaTime * fillSpeed;
        }


        if (handed)
        {
            handFillBar.fillAmount += Time.deltaTime * fillSpeed;

            if(handFillBar.fillAmount == 1)
            {
                swipePlayerController.handChosen = true;
                changeButton.SetActive(true);
                choosePanel.SetActive(false);
            }
        }
        else if (handFillBar.fillAmount != 0)
        {
            handFillBar.fillAmount -= Time.deltaTime * fillSpeed;
        }
    }

    private void OnDestroy()
    {
        _pisonEvents.OnExtension -= OnExtension;
    }

    #endregion
}
