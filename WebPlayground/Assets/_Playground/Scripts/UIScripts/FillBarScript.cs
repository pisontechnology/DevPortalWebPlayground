using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;
using UnityEngine.UI;

public class FillBarScript : MonoBehaviour
{
    private PisonEvents _pisonEvents;

    [SerializeField] private Image fillBar;
    [SerializeField] private float fillSpeed;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject selectionPanel;

    private bool startFillingBar = false;


    private void OnExtension(string gesture)
    {
        if(gesture == "HAND")
        {
            startFillingBar = true;
        }
        else
        {
            startFillingBar = false;
        }
    }

    void Awake()
    {
        selectionPanel.SetActive(false);

        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();

        _pisonEvents.OnExtension += OnExtension;

        fillBar.fillAmount = 0;
    }

    void OnDestroy()
    {
        _pisonEvents.OnExtension -= OnExtension;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startFillingBar = !startFillingBar;
        }

        if (startFillingBar)
        {
            fillBar.fillAmount += fillSpeed * Time.deltaTime;
        }
        else if(fillBar.fillAmount != 0)
        {
            fillBar.fillAmount -= fillSpeed * Time.deltaTime;
        }

        if(fillBar.fillAmount == 1)
        {
            selectionPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
    }
}
