using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PisonCore;
using com.pison;
using com.pison.wristgestures;


/// <summary>
/// Example of different programs that are possible with Pison gestures.
/// 
/// Program will allow user to draw using different colors then erase using the Pison Gesture system.
/// Along with allow the user to interact with a UI system using a gesture controlled cursor.
/// 
/// Written by: Joe Tecce (joe.tecce@pison.com), 2022
public class ExampleDrawManager : MonoBehaviour
{
    private enum BRUSH_STATE
    {
        DRAW,
        ERASE,
        DEFAULT
    };

    private enum COLOR_STATE
    {
        BLACK,
        BLUE,
        RED,
        DEFAULT
    };

    private PisonEvents pisonEvents;
    private PisonManager pisonManager;
    private PisonWristEvents pisonWristEvents;

    // Stroke Variables:
    private List<Vector2> currentStroke;
    private LineRenderer currentLineRenderer;
    private EdgeCollider2D currentEdgeCollider2d;

    private bool isDrawing = false;
    private bool isErasing = false;

    [SerializeField] private float strokeSeparationDist = 0.2f;
    [SerializeField] private int strokeCapVertices = 5;
    [SerializeField] private float strokeWidth = 0.1f;
    [HideInInspector] public Color strokeColor = Color.black;

    private Vector3 currentDrawPosition;
    private Vector3 zeroVector;

    [SerializeField] GameObject strokeManager;

    // UI Control Variables
    public RectTransform colorHighlight;
    public RectTransform brushHighlight;

    private COLOR_STATE colorState;
    private BRUSH_STATE brushState;
    private COLOR_STATE currentColorState = COLOR_STATE.BLACK;
    private BRUSH_STATE currentBrushState = BRUSH_STATE.DRAW;

    // Cursor Movement based Variables
    [Header("Cursor Movement")]
    public float cursorSensitivity = 0.6f;
    public float handDeadzoneThreashhold = 1f;

    private bool isCursorLocked = true;

    //public GameObject cursor;
    private Rigidbody2D cursorRb2d;

    // Vulken Control variables
    private int currentColor;
    private COLOR_STATE[] possibleColors = { COLOR_STATE.BLACK, COLOR_STATE.BLUE, COLOR_STATE.RED };

    #region Drawing Functions

    private IEnumerator Drawing()
    {
        isDrawing = true;

        // Actual start to drawing
        StartStroke();

        while (isDrawing)
        {
            // Adding the points where the line will draw along to an array
            AddPoint(transform.position);
            yield return null;
        }

        // Setting collider along the line just created
        // Will help with detecting eraesing later
        currentEdgeCollider2d.points = currentStroke.ToArray();
    }

    private void StartStroke()
    {
        // Create Stroke
        currentStroke = new List<Vector2>();
        GameObject currentLineObject = new GameObject();
        currentLineObject.name = "Stroke";
        currentLineObject.transform.parent = strokeManager.transform;
        currentLineRenderer = currentLineObject.AddComponent<LineRenderer>();
        currentEdgeCollider2d = currentLineObject.AddComponent<EdgeCollider2D>();

        // Setting Stroke varaibles
        currentLineRenderer.positionCount = 0;
        currentLineRenderer.startWidth = strokeWidth;
        currentLineRenderer.endWidth = strokeWidth;
        currentLineRenderer.numCapVertices = strokeCapVertices;
        currentLineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        currentLineRenderer.startColor = strokeColor;
        currentLineRenderer.endColor = strokeColor;
        currentEdgeCollider2d.edgeRadius = 0.1f;
    }

    private void AddPoint(Vector2 point)
    {
        // Adding points along stroke to create edgecolliders later
        if (PlacePoint(point))
        {
            currentStroke.Add(point);
            currentLineRenderer.positionCount++;
            currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, point);
        }
    }

    private bool PlacePoint(Vector2 point)
    {
        if (currentStroke.Count == 0)
            return true;

        // set distance for placement fo edgecollider and stroke
        if (Vector2.Distance(point, currentStroke[currentStroke.Count - 1]) < strokeSeparationDist)
            return false;

        return true;
    }

    #endregion


    #region Drawing States

    // Selecting the brush the user wants to use
    private void GetBrushState()
    {
        if (currentBrushState == BRUSH_STATE.DRAW)
        {
            if (isDrawing)
            {
                isDrawing = false;
            }
            else if (!isErasing)
            {
                StartCoroutine(Drawing());
            }
        }
        else if (currentBrushState == BRUSH_STATE.ERASE)
        {
            if (isErasing)
            {
                isErasing = false;
            }
            else 
            {
                isErasing = true;
            }
        }
    }

    // Select the color you want along with update the highlight UI
    private void CheckColorState()
    {
        // Check which color was selected
        switch (currentColorState)
        {
            case COLOR_STATE.BLACK:
                strokeColor = Color.black;
                colorHighlight.anchoredPosition = new Vector2(85.2f, -44.5f);
                break;

            case COLOR_STATE.BLUE:
                strokeColor = Color.blue;
                colorHighlight.anchoredPosition = new Vector2(85.2f, 15.5f);
                break;

            case COLOR_STATE.RED:
                strokeColor = Color.red;
                colorHighlight.anchoredPosition = new Vector2(85.2f, -14.5f);
                break;

            case COLOR_STATE.DEFAULT:
                break;
        }
    }

    // Select the brush you want along with update the highlight UI
    private void CheckBrushState()
    {
        // Check which brush was selected
        switch (currentBrushState)
        {
            case BRUSH_STATE.DRAW:
                brushHighlight.anchoredPosition = new Vector2(10f, 15.5f);

                // Deselects Erasing if you where erasing
                if (isErasing)
                    isErasing = false;
                break;

            case BRUSH_STATE.ERASE:
                brushHighlight.anchoredPosition = new Vector2(10f, -14.5f);

                // Deselect Drawing if you where drawing
                if (isDrawing)
                    isDrawing = false;
                break;

            case BRUSH_STATE.DEFAULT:
                break;
        }
    }

    #region Futor Vulken Functions

    private void SwitchBrushState()
    {
        // Switches between the different brushs
        switch (currentBrushState)
        {
            case BRUSH_STATE.DRAW:
                // Stops drawing
                if (isDrawing)
                    isDrawing = false;

                currentBrushState = BRUSH_STATE.ERASE;
                brushHighlight.anchoredPosition = new Vector2(10f, -14.5f);
                break;

            case BRUSH_STATE.ERASE:
                // Stops erasing 
                if (isErasing)
                    isErasing = false;

                currentBrushState = BRUSH_STATE.DRAW;
                brushHighlight.anchoredPosition = new Vector2(10f, 15.5f); 
                break;

            case BRUSH_STATE.DEFAULT:
                break;
        }
    }


    private void SwitchBrushColor(bool swipeUp)
    {
        COLOR_STATE selectedColor;

        // Switches between possible colors
        if (swipeUp)
        {
            currentColor++;
            currentColor = Mathf.Clamp(currentColor, 0, possibleColors.Length);
        }
        else if (!swipeUp)
        {
            currentColor--;
            currentColor = Mathf.Clamp(currentColor, 0, possibleColors.Length);
        }

        selectedColor = possibleColors[currentColor];

        /// <summary>
        /// Future will call CheckColorState() functions
        /// and add an in varaible of COLOR_STATE to call the same switch statement below
        /// </summary>

        // Selects desired color
        switch (selectedColor)
        {
            case COLOR_STATE.BLACK:
                strokeColor = Color.black;
                colorHighlight.anchoredPosition = new Vector2(85.2f, -44.5f);
                break;

            case COLOR_STATE.BLUE:
                strokeColor = Color.blue;
                colorHighlight.anchoredPosition = new Vector2(85.2f, 15.5f);
                break;

            case COLOR_STATE.RED:
                strokeColor = Color.red;
                colorHighlight.anchoredPosition = new Vector2(85.2f, -14.5f);
                break;
        }
    }

    #endregion

    #endregion


    #region Cursor Functions

    private void CursorMovementCheck()
    {
        // Getting data from device
        currentDrawPosition = pisonManager.currentDeviceEulerAngles - zeroVector;

        // Converting from vector 3 to vector 2
        Vector2 movement = new Vector2(currentDrawPosition.x, currentDrawPosition.y);

        // Stop movment from drasticly pulling right as data will come back 0 -> -30 then jump to 360 
        // This will catch that happening and correct it before any bug can occur
        if (movement.y > 180)
            movement = new Vector2(movement.x, movement.y - 360);

        //if(movement.y <-120)
        //    movement = new Vector2(movement.x, movement.y + 180);

        // Reversing the X and Y data collected to correct movment
        // Then multiplying X by -1 to switch - to + and + to - to create smooth movement
        Vector2 correctedMovement = new Vector2(movement.y, (movement.x * -1));

        // Applying movement to cursor
        cursorRb2d.MovePosition(cursorRb2d.position + correctedMovement * cursorSensitivity * Time.fixedDeltaTime);


        // Clamping position so Cursor can't go off the screen
        Vector3 clampedPosition = transform.position;

        Vector2 screenDimention = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        clampedPosition.y = Mathf.Clamp(transform.position.y, -screenDimention.y, screenDimention.y);
        clampedPosition.x = Mathf.Clamp(transform.position.x, -screenDimention.x, screenDimention.x);
        clampedPosition.z = 0;

        transform.localPosition = clampedPosition;
    }

    #endregion


    #region Pison Impl

    private void OnActivation(ActivationStates activation)
    {
        if(activation.Index == ActivationState.Hold)
        {
            // Zeroing to your arm
            zeroVector = pisonManager.currentDeviceEulerAngles;
            currentDrawPosition = pisonManager.currentDeviceEulerAngles - zeroVector;
        }
    }

    private void OnGesture(ImuGesture gesture)
    {
        switch (gesture)
        {
            case ImuGesture.IndexClick:
                // Setting current desired states
                if(brushState != BRUSH_STATE.DEFAULT)
                    currentBrushState = brushState;
                if (colorState != COLOR_STATE.DEFAULT)
                    currentColorState = colorState;
                
                CheckBrushState();
                CheckColorState();
                break;

            case ImuGesture.IndexHold:
                isCursorLocked = !isCursorLocked;
                break;
        }
    }

    private void OnWristGesture(WristGesture wristGesture)
    {
        // Start using selected brush
        if(wristGesture == WristGesture.WristFlickRight)
            GetBrushState();
    }

    /// <summary>
    /// Future Vulken gestures will go here specifically:
    /// - Full hand extention: To switch between brush options
    /// - Thumb swipe Up/Down: To swich between different brush color
    /// </summary>

    #endregion


    #region Unity Functions

    private void Start()
    {
        // Subscribing to correct Pison script events
        pisonManager = GameObject.FindObjectOfType<PisonManager>();
        pisonEvents = GameObject.FindObjectOfType<PisonEvents>();
        pisonWristEvents = GameObject.FindObjectOfType<PisonWristEvents>();

        pisonEvents.OnActivation += OnActivation;
        pisonEvents.OnGesture += OnGesture;
        pisonWristEvents.OnWristGesture += OnWristGesture;

        // Setting vector to be zeroed out
        currentDrawPosition = pisonManager.currentDeviceEulerAngles - Vector3.zero;

        cursorRb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isCursorLocked)
            CursorMovementCheck();
    }

    private void OnDestroy()
    {
        pisonEvents.OnGesture -= OnGesture;
        pisonEvents.OnActivation -= OnActivation;
        pisonWristEvents.OnWristGesture -= OnWristGesture;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // If collides with a stroke and set to erasing will destroy it
        if (collision.name == "Stroke" && isErasing)
        {
            Destroy(collision.gameObject);
        }

        // Tells which UI element you are hovering over
        switch (collision.name)
        {
            case "Button_Black":
                colorState = COLOR_STATE.BLACK;
                break;

            case "Button_Blue":
                colorState = COLOR_STATE.BLUE;
                break;

            case "Button_Red":
                colorState = COLOR_STATE.RED;
                break;

            case "Button_Draw":
                brushState = BRUSH_STATE.DRAW;
                break;

            case "Button_Erase":
                brushState = BRUSH_STATE.ERASE;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Turn off any state if exiting a UI element
        if(brushState != BRUSH_STATE.DEFAULT)
            brushState = BRUSH_STATE.DEFAULT;

        if(colorState != COLOR_STATE.DEFAULT)
            colorState = COLOR_STATE.DEFAULT;
    }

    #endregion
}
