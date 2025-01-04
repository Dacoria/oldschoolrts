using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RtsUnitManager : MonoBehaviour
{
    public static RtsUnitManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    private Vector3 StartClick;
    private Vector3 EndClick;
    private Vector3 StartMousePosition;

    public Image SelectionBox;
    public SquadBehaviour SelectionSquadPrefab;

    public SquadBehaviour TemporarySelectionSquad;
    public SquadBehaviour FixedSquad;

    public SquadBehaviour CurrentSelected => FixedSquad != null ? FixedSquad : TemporarySelectionSquad;
    
    public Dictionary<string, SquadBehaviour> FixedSquadsSelection = new Dictionary<string, SquadBehaviour>();

    private List<GameObject> units = new List<GameObject>();

    private GameObject squadsParentGo;

    private void Start()
    {
        squadsParentGo = GameObject.Find("Squads");
        TemporarySelectionSquad = Instantiate(SelectionSquadPrefab, squadsParentGo.transform);
    }

    private void Update()
    {
        int terrainMask = 1 << Constants.LAYER_TERRAIN;
        int unitMask = 1 << Constants.LAYER_RTS_UNIT;
        int wallMask = 1 << Constants.LAYER_WALL_LAYER;
        int terrainAndWallMask = terrainMask | wallMask;

        SelectUnitsWithMouseSelection(terrainMask, unitMask);

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, terrainAndWallMask))
            {
                //Debug.Log("Going to Pos: " + hit.point);
                CurrentSelected.SetDestination(hit.point);
            }
        }

        if (InputExtensions.TryGetNumberDown(out int number))
        {
            if (Input.GetKey(KeyCode.LeftAlt))
                TrySetNewFixedSquad(number);

            if (FixedSquadsSelection.ContainsKey(number.ToString()))
                FixedSquad = FixedSquadsSelection.GetValueOrDefault(number.ToString(), null);
        }
    }

    private void SelectUnitsWithMouseSelection(int terrainMask, int unitMask)
    {
        if (Input.GetMouseButtonDown(0))
        {
            FixedSquad = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity,
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

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity,
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
                }
            }
        }
    }

    private void TrySetNewFixedSquad(int number)
    {
        if (TemporarySelectionSquad == null || !TemporarySelectionSquad.Units.Any())
        {
            return;
        }

        if (FixedSquadsSelection.ContainsKey(number.ToString()))
        {
            Destroy(FixedSquadsSelection[number.ToString()]);
        }

        FixedSquadsSelection[number.ToString()] = TemporarySelectionSquad;
        TemporarySelectionSquad = Instantiate(SelectionSquadPrefab, squadsParentGo.transform);
    }
}