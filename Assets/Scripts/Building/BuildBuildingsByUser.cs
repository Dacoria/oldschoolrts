using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class BuildBuildingsByUser : MonoBehaviour
{
    private GameObject ObjectToBuild;
    private GameObject SelectedGameObjectToBuild; // hierin wordt het gameobject gezet wat wordt gebouwd. Hiermee wordt ObjectToBuild Initiated! (kan meerdere keren zijn door Roads)

    public bool useCollorSetter;
    public bool InstaBuild;

    private bool isHighlightedFieldShown => ObjectToBuild != null;
    private bool isMouseDragging;

    public GameObject BuildingProgressDisplayPrefab;
    public GameObject ProcessingDisplayPrefab;
    public GameObject FoodDisplayPrefab;

    private bool IsSelectedGoToBuildARoadOrField() => SelectedGameObjectToBuild?.name.IndexOf("Road") >= 0 || SelectedGameObjectToBuild?.name.IndexOf("FarmField") >= 0;

    private CheckCollisionForBuilding CheckCollisionForBuilding;

    private void Update()
    {
        if(SelectedGameObjectToBuild == null)
        {
            if (isHighlightedFieldShown) { Destroy(ObjectToBuild); }
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
            SelectedGameObjectToBuild = null;
            DestroyHighlightedObjects();
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDragging = false;
        }
    }

    private void DestroyHighlightedObjects()
    {
        Destroy(ObjectToBuild);

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
                if (!useCollorSetter || !CheckCollisionForBuilding.IsColliding())
                {
                    // alleen als het gebouw niet collide, dan bouwen
                    Build(ObjectToBuild);
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
        if ((!useCollorSetter || !CheckCollisionForBuilding.IsColliding()) && 
            ObjectToBuild != null &&
            LastKnownGhostRoadOrFieldLocation != null && 
            !ObjectToBuild.transform.position.IsSameVector3(LastKnownGhostRoadOrFieldLocation)
            )
        {
            // TODO: alleen als het gebouw/road niet collide, dan bouwen -> checken of dat al niet gebeurt (werkt CheckCollisionForBuilding?)
            IncreaseTemplateRoadOrField();
            LastKnownGhostRoadOrFieldLocation = ObjectToBuild.transform.position;
        }
    }

    private void IncreaseTemplateRoadOrField()
    {
        var rayCastLocation = Get2DRaycastLocation();
        if (rayCastLocation != null)
        {
            if(!RoadsOrFieldsHighlightedToBuild.Any(x => x.transform.position == LastKnownGhostRoadOrFieldLocation))
            {
                var roadOrFieldGO = Instantiate(SelectedGameObjectToBuild, LastKnownGhostRoadOrFieldLocation, Quaternion.identity);
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
            if(ObjectToBuild != null)
            { 
                LastKnownGhostRoadOrFieldLocation = ObjectToBuild.transform.position;  
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Debug.Log("Klik mouse 4 Road or Field");
            if (isHighlightedFieldShown)
            {
                if (!useCollorSetter || !AnyOfRoadsOrFieldsColliding())
                {
                    // alleen als het gebouw niet collide, dan bouwen
                    BuildRoadsOrFields(ObjectToBuild);
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
                LastKnownGhostRoadOrFieldLocation = ObjectToBuild.transform.position;
            }
        }
    }

    private bool AnyOfRoadsOrFieldsColliding()
    {
        if(CheckCollisionForBuilding.IsColliding())
        {
            return true;
        }

        foreach(var roadOrFieldGo in RoadsOrFieldsHighlightedToBuild)
        {
            var checkCol = roadOrFieldGo.GetComponent<CheckCollisionForBuilding>();
            if(checkCol == null || checkCol.IsColliding())
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateLocationHighlightField()
    {
        var rayCastLocation = Get2DRaycastLocation();
        if(rayCastLocation != null)
        {
            ObjectToBuild.transform.position = new Vector3(rayCastLocation.Item1, ObjectToBuild.transform.position.y, rayCastLocation.Item2);
        }
        else
        {
            // muis hovert niet over locatie
            //Debug.Log("UpdateLocationHighlightField -> muis hovert niet over locatie -> Destroy");
            DestroyHighlightedObjects();
        }
    }  

    private void ShowGhostBuilding()
    {
        ObjectToBuild = Instantiate(SelectedGameObjectToBuild, new Vector3(0, 0.01f, 0), Quaternion.identity);
        FillBuildingType(SelectedGameObjectToBuild, ObjectToBuild);

        if (useCollorSetter)
        {
            CheckCollisionForBuilding = ObjectToBuild.GetComponentInChildren<CheckCollisionForBuilding>(); // voor bepalen of collide wordt met ander iets
            if(CheckCollisionForBuilding != null)
            {
                CheckCollisionForBuilding.enabled = true;                
            }
        }

        var locOfResource = ObjectToBuild.GetComponentInChildren<ILocationOfResource>(true);
        if (locOfResource != null)
        {
            var rangedDisplay = Instantiate(MonoHelper.Instance.RangedDisplayPrefab, ObjectToBuild.GetComponentInChildren<GhostBuildingBehaviour>().transform);
            rangedDisplay.MaxRangeForResource = locOfResource.GetMaxRangeForResource();
            rangedDisplay.RangeType = locOfResource.GetRangeTypeToFindResource();
            rangedDisplay.gameObject.SetActive(true);
        }
    }

    private void FillBuildingType(GameObject PrefabToBuild, GameObject GoToBuild)
    {
        var buildingSettings = GameManager.Instance.BuildingPrefabItems.FirstOrDefault(x => x.BuildingPrefab == PrefabToBuild);
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

        ObjectToBuild = null; // anders blijft gebouw met je muis meebewegen
    }

    private void Build(GameObject goTemplate)
    {
        // alle checks of dit kan/mag zijn hiervoor gedaan
        var ghostBehaviour = goTemplate.GetComponentInChildren<GhostBuildingBehaviour>();
        if(ghostBehaviour != null)
        {            
            InitiateGhostBuilding(goTemplate);
        }
        
        if(goTemplate == ObjectToBuild)
        {
            ObjectToBuild = null;
        }
        SelectedGameObjectToBuild = null;
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
                var displayOffset = GameManager.Instance.BuildingPrefabItems.Single(x => x.BuildingPrefab == SelectedGameObjectToBuild).DisplayOffset;
                building.AddComponent<GenerateUpdateBuildingProgressDisplayResources>().BuildingProgressDisplayPrefab = BuildingProgressDisplayPrefab;
                building.GetComponent<GenerateUpdateBuildingProgressDisplayResources>().GoSpawnOffset = displayOffset;
                               
                buildingBehaviour.Real.AddComponent<GenerateUpdateInputOutputDisplayResources>().ProcessingDisplayPrefab = ProcessingDisplayPrefab;
                buildingBehaviour.Real.GetComponent<GenerateUpdateInputOutputDisplayResources>().GoSpawnOffset = displayOffset;

                Destroy(buildingBehaviour.GetComponentInChildren<RangeDisplayBehaviour>()?.gameObject);
            }
            else if (buildingBehaviour.gameObject.IsRoad())
            {
                building.AddComponent<GenerateUpdateBuildingProgressDisplayResources>().BuildingProgressDisplayPrefab = BuildingProgressDisplayPrefab;
                building.GetComponent<GenerateUpdateBuildingProgressDisplayResources>().GoSpawnOffset = new Vector3(0.8f, 0, 0.3f);
                building.GetComponent<GenerateUpdateBuildingProgressDisplayResources>().GoSpawnScaleOffset = new Vector3(0.85f, 0.85f, 0.85f);
            }
        }
    }

    public void BuildGeneric(GameObject go)
    {
        if (ObjectToBuild != null)
        {
            DestroyHighlightedObjects();
        }

        SelectedGameObjectToBuild = go;
    }  

    public void ClearSelectedGameObjectToBuild ()
    {
        // als niks gehighlight is, dan hoeft er ook niks gecleared te worden
        if (isHighlightedFieldShown)
        {
            if (ObjectToBuild != null)
            {
                DestroyHighlightedObjects();
            }           
        }

        SelectedGameObjectToBuild = null;
    }

    public void ToggleInstaBuild()
    {
        InstaBuild = !InstaBuild;
        //Debug.Log("Instabuild = " + InstaBuild);
        ClearSelectedGameObjectToBuild();
    }
    public void ToggleGetsHungry()
    {
        FoodConsumptionSettings.ToggleUseFoodConsumption_Active = !FoodConsumptionSettings.ToggleUseFoodConsumption_Active;
        ClearSelectedGameObjectToBuild();
    }
}