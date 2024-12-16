using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RtsUnitManager : MonoBehaviour
{
    private Vector3 StartClick;
    private Vector3 EndClick;
    private Vector3 StartMousePosition;

    public Image SelectionBox;
    public SquadBehaviour SelectionSquadPrefab;

    public SquadBehaviour TemporarySelectionSquad;
    public SquadBehaviour FixedSquad;

    public SquadBehaviour CurrentSelected
    {
        get
        {
            if (FixedSquad == null)
            {
                return TemporarySelectionSquad;
            }
            else
            {
                return FixedSquad;
            }
        }
    }
    public Dictionary<string, SquadBehaviour> Squads = new Dictionary<string, SquadBehaviour>();

    private List<GameObject> units = new List<GameObject>();

    // Start is called before the first frame update
    private void Start()
    {
        TemporarySelectionSquad = GameObject.Instantiate(SelectionSquadPrefab);
    }

    // Update is called once per frame
    private void Update()
    {
        int terrainMask = 1 << 3; // 3e layer
        int unitMask = 1 << 6;
        int wallMask = 1 << 7;
        int terrainAndWallMask = terrainMask | wallMask;
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            FixedSquad = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,
                terrainMask))
            {
                StartClick = hit.point;
            }

            StartMousePosition = Input.mousePosition;
        }

        SelectionBox.enabled = false;
        if (Input.GetMouseButton(0))
        {
            SelectionBox.enabled = true;
            var currentMousePosition = Input.mousePosition;
            var middle = (StartMousePosition + currentMousePosition) / 2;
            var dif = (StartMousePosition - currentMousePosition).Absolute();

            SelectionBox.gameObject.transform.position = middle;
            SelectionBox.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(dif.x, dif.y);

        }

        if (Input.GetMouseButtonUp(0))
        {
            TemporarySelectionSquad.Clear();

            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,
                terrainMask))
            {
                EndClick = hit.point;
                var middle = (EndClick + StartClick) / 2;
                var halfdif = ((EndClick - StartClick) / 2).Absolute();

                halfdif.y = 9999;

                var collisions = Physics.OverlapBox(middle, halfdif, Quaternion.identity, unitMask);

                foreach (var collision in collisions)
                {
                    TemporarySelectionSquad.AddUnit(collision.gameObject);
                    Debug.Log(collision.name);
                }
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity,
                    terrainAndWallMask))
            {
                //Debug.Log("Going to Pos: " + hit.point);
                CurrentSelected.SetDestination(hit.point);
            }

        }

        if (InputExtensions.GetNumberDown(out int number))
        {
            if (Input.GetKey("z"))
            {
                if (!Squads.ContainsKey(number.ToString()))
                {
                    Debug.Log("do the awesome");
                    Squads[number.ToString()] = TemporarySelectionSquad;
                    TemporarySelectionSquad = GameObject.Instantiate(SelectionSquadPrefab);
                }
            }

            FixedSquad = Squads[number.ToString()];
        }

    }
}