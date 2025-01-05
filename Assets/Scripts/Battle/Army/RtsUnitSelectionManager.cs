using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RtsUnitSelectionManager : MonoBehaviour
{
    public static RtsUnitSelectionManager Instance;
    public GameObject UISelectedArmyPanel;
    public GameObject UISelectedBuilding;

    private void Awake()
    {
        Instance = this;
    }

    private Vector3 StartClickLeft;
    private Vector3 EndClickLeft;
    private Vector3 StartMousePositionLeft;

    public Image SelectionBox;
    public SquadBehaviour SelectionSquadPrefab;

    public SquadBehaviour TemporarySelectionSquad;
    public SquadBehaviour FixedSquad;

    public SquadBehaviour CurrentSelected => FixedSquad != null ? FixedSquad : TemporarySelectionSquad;
    
    public Dictionary<string, SquadBehaviour> FixedSquadsSelection = new Dictionary<string, SquadBehaviour>();

    private List<GameObject> units = new List<GameObject>();

    private GameObject squadsParentGo;

    public CompassDirection DefaultDirection;

    private void Start()
    {
        squadsParentGo = GameObject.Find(Constants.GO_SCENE_SQUADS);
        TemporarySelectionSquad = Instantiate(SelectionSquadPrefab, squadsParentGo.transform);
    }

    private void Update()
    {
        HandleArmyPanelUI();

        var isClickingUi = EventSystem.current.IsPointerOverGameObject();
        if (isClickingUi)
        {
            return;
        }

        int terrainMask = 1 << Constants.LAYER_TERRAIN;
        int unitMask = 1 << Constants.LAYER_RTS_UNIT;
        int wallMask = 1 << Constants.LAYER_WALL_LAYER;
        int terrainAndWallMask = terrainMask | wallMask;

        SelectUnitsWithMouseSelection(terrainMask, unitMask);

        if (Input.GetMouseButtonUp(1))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, terrainAndWallMask))
            {
                CurrentSelected.Movement.SetDestination(hit.point);
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
                StartClickLeft = hit.point;
            }

            StartMousePositionLeft = Input.mousePosition;
        }

        SelectionBox.enabled = false;
        if (Input.GetMouseButton(0))
        {
            SelectionBox.enabled = true;
            var currentMousePosition = Input.mousePosition;
            var middle = (StartMousePositionLeft + currentMousePosition) / 2;
            var dif = (StartMousePositionLeft - currentMousePosition).Absolute();

            SelectionBox.gameObject.transform.position = middle;
            SelectionBox.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(dif.x, dif.y);
        }

        if (Input.GetMouseButtonUp(0))
        { 
            TemporarySelectionSquad.Clear();

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity,
                terrainMask))
            {
                EndClickLeft = hit.point;
                var middle = (EndClickLeft + StartClickLeft) / 2;
                var halfdif = ((EndClickLeft - StartClickLeft) / 2).Absolute();

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
        if (TemporarySelectionSquad == null || !TemporarySelectionSquad.GetUnits().Any())
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

    private void HandleArmyPanelUI()
    {
        // building selectie actief? Dan geen squad dingen doen
        if (UISelectedBuilding.activeSelf)
        {
            FixedSquad = null;
            TemporarySelectionSquad.Clear();
        }
        
        var isClickingUi = EventSystem.current.IsPointerOverGameObject();
        if (isClickingUi)
        {
            return;
        }
        UISelectedArmyPanel.SetActive(CurrentSelected.GetUnits().Count > 0);
    }
}