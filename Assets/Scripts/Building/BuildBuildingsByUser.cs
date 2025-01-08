using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingsByUser : MonoBehaviour
{
    public static BuildBuildingsByUser Instance;

    private void Awake()
    {
        Instance = this;    
    }

    private GameObject objectToBuild;
    private GameObject selectedGameObjectToBuild; // hierin wordt het gameobject gezet wat wordt gebouwd. Hiermee wordt ObjectToBuild Initiated! (kan meerdere keren zijn door Roads)

    [HideInInspector] public bool InstaBuild = false;

    private bool isHighlightedFieldShown => objectToBuild != null;
    private bool isMouseDragging;

    private DisplayProcessingInputOutput displayProcessingInputOutputPrefab;

    private bool IsSelectedGoToBuildARoadOrField() => selectedGameObjectToBuild.IsRoad() || selectedGameObjectToBuild.IsFarmField();

    private CheckCollisionHandler checkCollisionForBuilding;

    private void Start()
    {
        displayProcessingInputOutputPrefab = Load.GoMapBuildings[Constants.GO_PREFAB_UI_PROCESSING_DISPLAY_INPUT_OUTPUT].GetComponent<DisplayProcessingInputOutput>();
    }

    private void Update()
    {
        if(selectedGameObjectToBuild == null)
        {
            if (isHighlightedFieldShown) { Destroy(objectToBuild); }
        }
        else if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonUp(0))
        {
            if (IsSelectedGoToBuildARoadOrField())
            {
                UpdateLeftMouseInputForRoadOrField();
            }
            else
            {
                UpdateLeftMouseInputForBuildings();
            }
        }
        else if(isMouseDragging && isHighlightedFieldShown && IsSelectedGoToBuildARoadOrField())
        {
            UpdateDragLeftMouseInputForRoadOrField();
        }
        
        if(isHighlightedFieldShown)
        {
            UpdateLocationHighlightField();
        }

        if (Input.GetMouseButtonUp(1)) // rechter muisknop
        {
            selectedGameObjectToBuild = null;
            DestroyHighlightedObjects();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
        }
    }

    private void DestroyHighlightedObjects()
    {
        Destroy(objectToBuild);

        // alleen voor roads
        if (RoadsOrFieldsHighlightedToBuild.Any())
        {
            foreach (var roadToBuild in RoadsOrFieldsHighlightedToBuild)
            {
                Destroy(roadToBuild);
            }
            RoadsOrFieldsHighlightedToBuild = new List<GameObject>();
        }
    }

    private void UpdateLeftMouseInputForBuildings()
    {
        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Klik mouse 4 buildigs");
            if (isHighlightedFieldShown)
            {
                if (!checkCollisionForBuilding.IsColliding())
                {
                    // alleen als het gebouw niet collide, dan bouwen
                    Build(objectToBuild);
                    ClearSelectedGameObjectToBuild();
                }
            }
            else
            {
                ShowGhostBuilding();
            }
        }        
    }

    private void UpdateDragLeftMouseInputForRoadOrField()
    {                 
        if (!checkCollisionForBuilding.IsColliding() && 
            objectToBuild != null &&
            LastKnownGhostRoadOrFieldLocation != null && 
            !objectToBuild.transform.position.IsSameVector3(LastKnownGhostRoadOrFieldLocation)
            )
        {
            // TODO: alleen als het gebouw/road niet collide, dan bouwen -> checken of dat al niet gebeurt (werkt CheckCollisionForBuilding?)
            IncreaseTemplateRoadOrField();
            LastKnownGhostRoadOrFieldLocation = objectToBuild.transform.position;
        }
    }

    private void IncreaseTemplateRoadOrField()
    {
        var rayCastLocation = Get2DRaycastLocation();
        if (rayCastLocation != null)
        {
            if(!RoadsOrFieldsHighlightedToBuild.Any(x => x.transform.position == LastKnownGhostRoadOrFieldLocation))
            {
                var roadOrFieldGO = Instantiate(selectedGameObjectToBuild, LastKnownGhostRoadOrFieldLocation, Quaternion.identity);
                RoadsOrFieldsHighlightedToBuild.Add(roadOrFieldGO);
            }
        }
    }

    private Vector3 LastKnownGhostRoadOrFieldLocation; // om te voorkomen dat je op 1 square meerdere roads bouwt
    private List<GameObject> RoadsOrFieldsHighlightedToBuild = new List<GameObject>();


    private void UpdateLeftMouseInputForRoadOrField()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isMouseDragging = true;
            if(objectToBuild != null)
            { 
                LastKnownGhostRoadOrFieldLocation = objectToBuild.transform.position;  
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Klik mouse 4 Road or Field");
            if (isHighlightedFieldShown)
            {
                if (!checkCollisionForBuilding.IsColliding())
                {
                    // alleen als het gebouw niet collide, dan bouwen
                    BuildRoadsOrFields(objectToBuild);
                }
                else
                {
                    DestroyHighlightedObjects();
                }
            }
            else
            {
                ShowGhostBuilding();
                UpdateLocationHighlightField();
                LastKnownGhostRoadOrFieldLocation = objectToBuild.transform.position;
            }
        }
    }

    private void UpdateLocationHighlightField()
    {
        var rayCastLocation = Get2DRaycastLocation();
        if(rayCastLocation != null)
        {
            objectToBuild.transform.position = new Vector3(rayCastLocation.Item1, objectToBuild.transform.position.y, rayCastLocation.Item2);
        }
        else
        {
            // muis hovert niet over locatie
            DestroyHighlightedObjects();
        }
    }  

    private void ShowGhostBuilding()
    {
        objectToBuild = Instantiate(selectedGameObjectToBuild, new Vector3(0, 0.01f, 0), Quaternion.identity);
        FillBuildingType(selectedGameObjectToBuild, objectToBuild);

        checkCollisionForBuilding = objectToBuild.AddComponent<CheckCollisionHandler>(); // voor bepalen of collide wordt met ander iets

        var locOfResource = objectToBuild.GetComponentInChildren<ILocationOfResource>(true);
        if (locOfResource != null)
        {
            var rangedDisplayGo = Instantiate(Load.GoMapUI[Constants.GO_PREFAB_UI_RANGE_DISPLAY], objectToBuild.GetComponentInChildren<GhostBuildingBehaviour>().transform);
            var rangedDisplay = rangedDisplayGo.GetComponent<RangeDisplayBehaviour>();
            rangedDisplay.MaxRangeForResource = locOfResource.GetMaxRangeForResource();
            rangedDisplay.RangeType = locOfResource.GetRangeTypeToFindResource();
            rangedDisplay.gameObject.SetActive(true);
        }
    }

    private void FillBuildingType(GameObject PrefabToBuild, GameObject GoToBuild)
    {
        var buildingSettings = BuildingPrefabs.Get().FirstOrDefault(x => x.BuildingPrefab == PrefabToBuild);
        if(buildingSettings != null)
        {
            var buildingBehaviour = GoToBuild.GetComponent<BuildingBehaviour>();
            if(buildingBehaviour != null)
            {
                buildingBehaviour.BuildingType = buildingSettings.BuildingType;
            }
        }
    }

    private Tuple<int, int> Get2DRaycastLocation()
    {
        int terrainMask = 1 << 3; // 3e layer
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, terrainMask))
        {
            var xRounded = (int)Math.Round(hit.point.x) ;
            var zRounded = (int)Math.Round(hit.point.z) ;

            return new Tuple<int, int>(xRounded, zRounded);
        }

        return null;    
    }

    private void BuildRoadsOrFields(GameObject lastRoadOrFieldTemplate)
    {
        var ghostBehaviour = lastRoadOrFieldTemplate.GetComponentInChildren<GhostBuildingBehaviour>();
        if (ghostBehaviour != null)
        {
            foreach(var roadOrFieldToBuild in RoadsOrFieldsHighlightedToBuild)
            {
                Build(roadOrFieldToBuild);
            }
            RoadsOrFieldsHighlightedToBuild.Clear();
            Build(lastRoadOrFieldTemplate);
        }

        objectToBuild = null; // anders blijft gebouw met je muis meebewegen
    }

    private void Build(GameObject goTemplate)
    {
        // alle checks of dit kan/mag zijn hiervoor gedaan
        var ghostBehaviour = goTemplate.GetComponentInChildren<GhostBuildingBehaviour>();
        if(ghostBehaviour != null)
        {            
            InitiateGhostBuilding(goTemplate);
        }
        
        if(goTemplate == objectToBuild)
        {
            objectToBuild = null;
        }
        selectedGameObjectToBuild = null;
    }

    private void InitiateGhostBuilding(GameObject building)
    {
        var buildingBehaviour = building.GetComponent<BuildingBehaviour>();
        if (buildingBehaviour != null)
        {
            buildingBehaviour.DEBUG_RealImmediately = InstaBuild;
            buildingBehaviour.ActivateGhost();

            if (!buildingBehaviour.gameObject.IsRoad() && !buildingBehaviour.gameObject.IsFarmField())
            {
                var displayOffset = BuildingPrefabs.Get().Single(x => x.BuildingPrefab == selectedGameObjectToBuild).DisplayOffset;
                               
                buildingBehaviour.Real.AddComponent<DisplayBuildingInputOutputHandler>().DisplayProcessingInputOutputPrefab = displayProcessingInputOutputPrefab;
                buildingBehaviour.Real.GetComponent<DisplayBuildingInputOutputHandler>().GoSpawnOffset = displayOffset;

                Destroy(buildingBehaviour.GetComponentInChildren<RangeDisplayBehaviour>()?.gameObject);
            }            
        }
    }

    public void BuildGeneric(GameObject go)
    {
        if (objectToBuild != null)
        {
            DestroyHighlightedObjects();
        }

        selectedGameObjectToBuild = go;
    }  

    public void ClearSelectedGameObjectToBuild ()
    {
        // als niks gehighlight is, dan hoeft er ook niks gecleared te worden
        if (isHighlightedFieldShown)
        {
            if (objectToBuild != null)
            {
                DestroyHighlightedObjects();
            }           
        }

        selectedGameObjectToBuild = null;
    }
}