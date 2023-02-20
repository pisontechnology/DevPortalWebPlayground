using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.pison;

public class DemoGestureSelection : MonoBehaviour
{
    private PisonEvents _pisonEvents;
    private GestureHold holdScript;

    [SerializeField] private DemoManager demoManager;

    public bool lockMoveState;
    private bool indexed;
    private bool clicked;

    [SerializeField] private GameObject activeOutline;
    [SerializeField] private GameObject activePointer;
    [SerializeField] private GameObject DeactiveOutline;
    [SerializeField] private GameObject DeactivePointer;

    private float[] selectionPosition = new float[4] { 158.7f, 68.7f, -21f, -110.7f };
    public int currentPosition = 0;

    [SerializeField] private float holdTime;
    [SerializeField] private float clickTime;
    [SerializeField] private float timeToClick;


    #region Public Functions

    public void ChangeMoveLockState(bool newLockState)
    {
        lockMoveState = newLockState;

        if (lockMoveState)
        {
            activeOutline.SetActive(false);
            activePointer.SetActive(false);
            DeactiveOutline.SetActive(true);
            DeactivePointer.SetActive(true);
        }
        else
        {
            activeOutline.SetActive(true);
            activePointer.SetActive(true);
            DeactiveOutline.SetActive(false);
            DeactivePointer.SetActive(false);
        }
    }

    public void UpdatePosition(int desiredPosition)
    {
        currentPosition = desiredPosition;

        transform.localPosition = new Vector3(transform.localPosition.x, selectionPosition[currentPosition], 0);
    }

    #endregion

    #region Private Functions

    private void MoveUpward()
    {
        if(currentPosition > 0)
        {
            currentPosition--;

            transform.localPosition = new Vector3(transform.localPosition.x, selectionPosition[currentPosition], 0);
        }
    }

    private void MoveDownward()
    {
        if(currentPosition < selectionPosition.Length)
        {
            currentPosition++;

            transform.localPosition = new Vector3(transform.localPosition.x, selectionPosition[currentPosition], 0);
        }
    }

    #endregion

    #region Pison Functions

    private IEnumerator DoubleClickTimer()
    {
        yield return new WaitForSeconds(timeToClick);

        clicked = false;
    }

    private IEnumerator ClickSequence()
    {
        yield return new WaitForSeconds(clickTime);

        if (!indexed)
        {
            Debug.Log("clicked");

            if (!clicked)
            {
                clicked = true;
                StartCoroutine(DoubleClickTimer());
            }
            else
            {
                Debug.Log("Double Click");
                lockMoveState = !lockMoveState;
                ChangeMoveLockState(lockMoveState);
            }
        }
    }

    private IEnumerator HoldSequence()
    {
        yield return new WaitForSeconds(holdTime);

        if (indexed)
        {
            demoManager.SwitchDemo(currentPosition);
        }
    }

    private void OnExtension(string gesture)
    {
        Debug.Log(gesture);

        if (!lockMoveState)
        {

            switch (gesture)
            {
                case "INDEX_SWIPE_UP":
                    indexed = false;
                    MoveUpward();
                    break;
                case "INDEX_SWIPE_DOWN":
                    indexed = false;
                    MoveDownward();
                    break;
                case "INDEX":
                    indexed = true;
                    StartCoroutine(HoldSequence());
                    StartCoroutine(ClickSequence());
                    break;
                default:
                    indexed = false;
                    break;
            }
        }
        else
        {
            if(gesture == "INDEX")
            {
                indexed = true;
                StartCoroutine(ClickSequence());
            }
            else
            {
                indexed = false;
            }
        }
    }

    #endregion

    #region Unity Function

    void Awake()
    {
        _pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        holdScript = GetComponent<GestureHold>();

        _pisonEvents.OnExtension += OnExtension;

        ChangeMoveLockState(false);
    }

    private void Update()
    {
        // FOR PROTOTYPE **************
        if (Input.GetKeyDown(KeyCode.L))
        {
            lockMoveState = !lockMoveState;
            ChangeMoveLockState(lockMoveState);
        }
    }

    private void OnDestroy()
    {
        _pisonEvents.OnExtension -= OnExtension;
    }

    #endregion
}
