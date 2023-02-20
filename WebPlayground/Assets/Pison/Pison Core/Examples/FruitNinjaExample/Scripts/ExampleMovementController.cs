using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PisonCore;
using com.pison;
using com.pison.wristgestures;


/// <summary>
/// 
/// Handels cursor movement up, down, left, right using Pison Impl, Also sets up move locations to work with any screen size
/// Handels calling Slash using Pison Impl
/// Controls Sword spirte changing with indexActivation
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleMovementController : MonoBehaviour
{
    private PisonManager pisonManager;
    private PisonEvents pisonEvents;
    private PisonWristEvents pisonWristEvents;

    private Transform cursor;

    [Header("Movement Variables: ")]
    [SerializeField] private float screenOffset;
    [SerializeField] private float screenOffsetY;

    private List<float> colums = new List<float>(new float[4]);
    private List<float> rows = new List<float>(new float[3]);

    private int currentColum;
    private int currentRow;

    [Header("Slash Variables: ")]
    [SerializeField] private float slashDuration;
    [SerializeField] private GameObject horizontalSlash;
    [SerializeField] private GameObject verticalSlash;
    private bool isSlashing;

    [Header("Sprite Variables: ")]
    [SerializeField] private Sprite basicSaber;
    [SerializeField] private Sprite activeSaber;
    private SpriteRenderer spriteRenderer;


    #region Slash Functions

    // Spawns horizontal slash prefab and stops all movement and other slashing
    private IEnumerator PerformHorizontalSlash()
    {
        isSlashing = true;
        GameObject slash = Instantiate(horizontalSlash, transform);

        yield return new WaitForSeconds(slashDuration);

        slash.GetComponent<ExampleSlash>().SlashEnd(true);
        isSlashing = false;
    }

    // Spawns veritcal slash prefab and stops all movement and other slashing
    private IEnumerator PerformVerticalSlash()
    {
        isSlashing = true;
        GameObject slash = Instantiate(verticalSlash, transform);

        yield return new WaitForSeconds(slashDuration);

        slash.GetComponent<ExampleSlash>().SlashEnd(true);
        isSlashing = false;
    }

    #endregion


    #region Movement Functions

    // Moves desired Column
    private void MoveColumn(bool isRight)
    {
        if (isRight)
        {
            currentColum++;
        }
        else
        {
            currentColum--;
        }

        currentColum = Mathf.Clamp(currentColum, 0, colums.Count - 1);
        cursor.position = new Vector3(colums[currentColum], cursor.position.y, cursor.position.z);
        //Debug.Log("Current Column = " + currentColum);
    }

    // Moves desired Row
    private void MoveRow(bool isUp)
    {
        if (isUp)
        {
            currentRow++;
        }
        else
        {
            currentRow--;
        }

        currentRow = Mathf.Clamp(currentRow, 0, rows.Count - 1);
        cursor.position = new Vector3(cursor.position.x, rows[currentRow], cursor.position.z);
        //Debug.Log("Current Row = " + currentRow);
    }

    private void SetupMoveCords()
    {
        cursor = transform;

        // Setting rows and collums to work on any screen size/shape
        float halfWidthScreen = Screen.width / 2f;
        float quaterWidthScreen = Screen.width / 4f;
        float quaterHightScreen = Screen.height / 4f;

        Vector3 camera1 = Camera.main.transform.position;

        camera1.x = halfWidthScreen;

        Camera.main.transform.position = new Vector3(camera1.x, camera1.y, camera1.z);

        Vector3 pos1 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen, quaterHightScreen, 0));
        Vector3 pos2 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 2, quaterHightScreen * 2, 0));
        Vector3 pos3 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 3, quaterHightScreen * 3, 0));
        Vector3 pos4 = Camera.main.ScreenToWorldPoint(new Vector3(quaterWidthScreen * 4, quaterHightScreen * 4, 0));

        colums[0] = pos1.x + screenOffset;
        colums[1] = pos2.x + screenOffset;
        colums[2] = pos3.x + screenOffset;
        colums[3] = pos4.x + screenOffset;

        rows[0] = pos1.y + screenOffsetY;
        rows[1] = pos2.y + screenOffsetY;
        rows[2] = pos3.y + screenOffsetY;
        //rows[3] = pos4.y + screenOffsetY;

        // Default position of curosr
        cursor.position = new Vector3(colums[0], rows[0], 0f);

        //Debug.Log("Columns: " + colums.Count);
        //Debug.Log("Rows: " + rows.Count);
    }

    #endregion


    #region Pison Impl

    private void OnGesture(ImuGesture gesture)
    {
        // Get swipe input for vertical and horizontal slashs
        if (!isSlashing)
        {
            switch (gesture)
            {
                case ImuGesture.SwipeUp:
                    StartCoroutine(PerformVerticalSlash());
                    break;
                case ImuGesture.SwipeDown:
                    StartCoroutine(PerformVerticalSlash());
                    break;

                case ImuGesture.SwipeRight:
                    StartCoroutine(PerformHorizontalSlash());
                    break;
                case ImuGesture.SwipeLeft:
                    StartCoroutine(PerformHorizontalSlash());
                    break;
            }
        }
    }

    private void OnWristGesture(WristGesture wristGesture)
    {
        // Controls movement of cursor
        if (!pisonManager.indexActivated && !isSlashing)
        {
            if (wristGesture == WristGesture.WristFlickRight)
            {
                MoveColumn(true); // Move right on the colums
            }
            else if (wristGesture == WristGesture.WristFlickLeft)
            {
                MoveColumn(false); // Move Left on the colums
            }
        }
        else if(pisonManager.indexActivated && !isSlashing)
        {
            if (wristGesture == WristGesture.WristFlickRight)
            {
                MoveRow(true); // Move up on the rows
            }
            else if (wristGesture == WristGesture.WristFlickLeft)
            {
                MoveRow(false); // Move down on the rows
            }
        }
        
        // Starts game if any wrist gesture is noticed
        if (!ExampleFruitNinjaGameManager.instance.gameStart)
        {
            // Any flick will start the game
            if (wristGesture == WristGesture.WristFlickRight || 
                wristGesture == WristGesture.WristFlickLeft || 
                wristGesture == WristGesture.WristShake)
            {
                ExampleFruitNinjaGameManager.instance.StartGame();
            }
        }
    }

    #endregion


    #region Unity Functions

    private void Awake()
    {
        pisonManager = GameObject.FindObjectOfType<PisonManager>();
        pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        pisonWristEvents = GameObject.FindObjectOfType<PisonWristEvents>();

        pisonEvents.OnGesture += OnGesture;
        pisonWristEvents.OnWristGesture += OnWristGesture;

        spriteRenderer = GetComponent<SpriteRenderer>();

        // Sets up the rows and columns for the cursor to move between
        SetupMoveCords();
    }

    private void Update()
    {
        // Change saber sprite color
        if (pisonManager.indexActivated)
        {
            spriteRenderer.sprite = activeSaber;
        }
        else
        {
            spriteRenderer.sprite = basicSaber;
        }

        /*
        // Keyboard Controls:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveColumn(false); // Move Left on the colums

        if(Input.GetKeyDown(KeyCode.RightArrow))
            MoveColumn(true); // Move right on the colums

        if(Input.GetKeyDown(KeyCode.UpArrow))
            MoveRow(true); // Move up on the rows

        if(Input.GetKeyDown(KeyCode.DownArrow))
            MoveRow(false); // Move down on the rows

        if (Input.GetKeyDown(KeyCode.F) && !isSlashing)
            StartCoroutine(PerformHorizontalSlash());

        if(Input.GetKeyDown(KeyCode.V) && !isSlashing)
            StartCoroutine(PerformVerticalSlash());

        if (Input.GetKeyDown(KeyCode.Space) && !ExampleFruitNinjaGameManager.instance.gameStart)
            ExampleFruitNinjaGameManager.instance.StartGame();

        if (Input.GetKeyDown(KeyCode.K))
            ExampleFruitNinjaGameManager.instance.TakeDamage(1);
        */
    }

    private void OnDestroy()
    {
        pisonEvents.OnGesture -= OnGesture;
        pisonWristEvents.OnWristGesture -= OnWristGesture;
    }

    #endregion
}
