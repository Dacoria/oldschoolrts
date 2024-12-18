using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    // gebruik = WASD, middelste muisknop indrukken voor rondkijken, en scrollen voor zoomen
    public float mainSpeed = 100; //regular speed
    public float shiftAdd = 5; //multiplied by how long shift is held.  Basically running
    public float maxShift = 50; //Maximum speed when holdin gshift
    public float scrollSpeed = 20; //Maximum speed when holdin gshift
    public float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    private bool mouseIsHeldDown = false;

    private void Start()
    {
        lastMouse = Input.mousePosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2)) // middelste muisknop
        {
            mouseIsHeldDown = true;
            lastMouse = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            mouseIsHeldDown = false;
        }

        if (mouseIsHeldDown)
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;
        }

        //Keyboard commands
        Vector3 changeInputKeys = GetBaseInput();
        var changeInput = SetSpeedOfChange(changeInputKeys);

        Vector3 positionBeforeChange = transform.position;

        //Only move on X and Z axis only with keys        
        transform.Translate(changeInput);
        positionBeforeChange.x = transform.position.x;
        positionBeforeChange.z = transform.position.z;
        transform.position = positionBeforeChange;

        //scrollen = zoomen
        positionBeforeChange = transform.position;

        Vector3 changeInputScrollingWheel = GetScrollInput();
        var changeInputScrolling = SetSpeedOfChange(changeInputScrollingWheel);

        transform.Translate(changeInputScrolling);
        if(transform.position.y < 1) // zorgt scrolling dat je onder de 1 komt op de y as? terugzetten oude pos (te dicht bij grond)
        {
            transform.position = positionBeforeChange;
        }

    }

    private Vector3 SetSpeedOfChange(Vector3 changeInput)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            changeInput = changeInput * totalRun * shiftAdd / 5;
            changeInput.x = Mathf.Clamp(changeInput.x, -maxShift, maxShift);
            changeInput.y = Mathf.Clamp(changeInput.y, -maxShift, maxShift);
            changeInput.z = Mathf.Clamp(changeInput.z, -maxShift, maxShift);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.1f, 1f, 10);
            changeInput = changeInput * mainSpeed / 5;
        }

        return changeInput * Time.deltaTime; // waarde
    }

    private Vector3 GetScrollInput()
    {
        Vector3 p_Velocity = new Vector3();


        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            p_Velocity += new Vector3(0, 0, scrollSpeed);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            p_Velocity += new Vector3(0, 0, -scrollSpeed);
        }
        
        return p_Velocity;

    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();


        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }

        return p_Velocity;
    }
}