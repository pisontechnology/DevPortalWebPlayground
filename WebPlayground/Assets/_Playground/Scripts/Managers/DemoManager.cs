using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoManager : MonoBehaviour
{
    /*
     * 0: Gesture Demo
     * 1: Swipe Demo
     * 2: Shake Demo
     * 3: IMU Demo
     * 4: Raw Data
    */
    [SerializeField] private GameObject[] demoObjects;

    [SerializeField] private DemoGestureSelection demoGestureSelection;

    private GameObject activeDemo;

    private bool rawDemoActive;


    #region Public Functions

    public void SwitchDemo(int desiredDemo)
    {
        /*
        // Make sure all demos have been turned off
        for(int i = 0; i < demoObjects.Length; ++i)
        {
            demoObjects[i].SetActive(false);
        }*/

        if (activeDemo != null)
            Destroy(activeDemo);

        if (rawDemoActive)
            demoObjects[4].SetActive(false); rawDemoActive = false;
       
        
        switch (desiredDemo)
        {
            case 0:
                //demoObjects[0].SetActive(true);
                activeDemo = Instantiate(demoObjects[0]);
                demoGestureSelection.UpdatePosition(0);
                break;
            case 1:
                //demoObjects[1].SetActive(true);
                activeDemo = Instantiate(demoObjects[1]);
                demoGestureSelection.UpdatePosition(1);
                break;
            case 2:
                //demoObjects[2].SetActive(true);
                activeDemo = Instantiate(demoObjects[2]);
                demoGestureSelection.UpdatePosition(2);
                break;
            case 3:
                Debug.Log("Hi");
                //demoObjects[3].SetActive(true);
                activeDemo = Instantiate(demoObjects[3]);
                demoGestureSelection.UpdatePosition(3);
                break;
            case 4:
                Debug.Log("Sup");
                rawDemoActive = true;
                demoObjects[4].SetActive(true);
                demoGestureSelection.UpdatePosition(4);
                break;
            default:
                Debug.LogError("Demo does not excist; check demoObjects, or number of desired demo");
                break;
        }
    }

    #endregion

    #region Unity Functions

    void Awake()
    {
        /*
        // Make sure all demos have been turned off
        for (int i = 0; i < demoObjects.Length; ++i)
        {
            demoObjects[i].SetActive(false);
        }*/
    }

    #endregion
}
