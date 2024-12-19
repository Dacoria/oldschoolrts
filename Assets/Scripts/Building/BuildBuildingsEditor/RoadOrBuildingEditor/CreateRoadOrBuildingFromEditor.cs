using System;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class CreateRoadOrBuildingFromEditor: MonoBehaviour
{
    public Camera MainCamera;         
    public GameObject RoadPrefab;
    public NavMeshSurface NavMeshSurfaceParent;
    public GameManager GameManager;
    public DisplayProcessingInputOutput DisplayProcessingInputOutputPrefab;

    private GameObject FirstBuildingOrRoadSelected;
    private GameObject SecondRoadSelected;


    // editor --> bij opnieuw compileren wordt dit altijd aangeroepen -> cleanup
    private void OnEnable()
    {        
        RoadPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/Final-Prefab/Road/Road.prefab", typeof(GameObject));

        var editorRoads = FindObjectsOfType<EditorBuildBehaviour>();
        for (int i = editorRoads.Length - 1; i >= 0; i--)
        {
            DestroyImmediate(editorRoads[i].gameObject);
        }
        FirstBuildingOrRoadSelected = null;
        SecondRoadSelected = null;        
    }

    private bool BuildEditorActive => EditorSettings.BuildBuildingOnMouseActivity_Active || EditorSettings.BuildRoadOnMouseActivity_Active;

    private BuildingPrefabItem GetBuildingPrefabItem(BuildingType buildingType)
    {
        return GameManager.BuildingPrefabItems.FirstOrDefault(x => x.BuildingType == buildingType);        
    }

    private void OnGUI()
    {
        if(Application.isPlaying && FirstBuildingOrRoadSelected != null)
        {
            DestroyEditorBuildings();
            return;
        }

        if (!Application.isPlaying && BuildEditorActive)
        {
            if (Event.current.isMouse && Event.current.type == EventType.MouseUp)
            {
                // flip, want editor!
                var mousePosition = Event.current.mousePosition;
                mousePosition.y = Screen.height - (mousePosition.y + 10);

                if (Event.current.button == 0)
                {
                    HandleLeftMouseClick(mousePosition);
                }
                else if (Event.current.button == 1)
                {
                    HandleRightMouseClick();
                }
            }
        }
    }

    private void HandleRightMouseClick()
    {
        DestroyEditorBuildings();
    }

    private void DestroyEditorBuildings()
    {
        if (FirstBuildingOrRoadSelected != null)
        {
            DestroyImmediate(FirstBuildingOrRoadSelected);
        }
        if (SecondRoadSelected != null)
        {
            DestroyImmediate(SecondRoadSelected);
        }
    }

    private void HandleLeftMouseClick(Vector2 mousePosition)
    {
        var v3Mouse = MainCamera.ScreenToWorldPoint(mousePosition);
        v3Mouse.y = 0.01f;

        if(ClickOnEditorRoadOrBuilding(mousePosition))
        {
            FinalizeRoadOrBuilding();
        }
        else
        {
            if(EditorSettings.BuildBuildingOnMouseActivity_Active 
                && FirstBuildingOrRoadSelected == null)
            {                
                InitializeNewRoadOrBuilding(mousePosition);                
            }
            else if (EditorSettings.BuildRoadOnMouseActivity_Active &&
                    (FirstBuildingOrRoadSelected == null || SecondRoadSelected == null)
                )
            {               
                InitializeNewRoadOrBuilding(mousePosition);                
            }
            else
            {
                DestroyEditorBuildings();
            }
        }
    }

    private GameObject InstantiateRoadGo(Vector3 position)
    {
        var roadSelected = PrefabUtility.InstantiatePrefab(RoadPrefab) as GameObject;
        roadSelected.name = "Road_To_Place";
        roadSelected.AddComponent<EditorBuildBehaviour>();
        roadSelected.transform.position = position;

        return roadSelected;
    }

    private GameObject InstantiateBuildingGo(Vector3 position)
    {
        var buildingPrefab = GetBuildingPrefabItem(EditorSettings.SelectedBuildingType.Value).BuildingPrefab;
        var buildingSelected = PrefabUtility.InstantiatePrefab(buildingPrefab) as GameObject;
        buildingSelected.name = EditorSettings.SelectedBuildingType.Value.ToString().Capitalize() + "_To_Place";
        buildingSelected.AddComponent<EditorBuildBehaviour>();
        buildingSelected.transform.position = position;

        return buildingSelected;
    }

    private void InitializeNewRoadOrBuilding(Vector2 mousePosition)
    {
        var v3Mouse = GetVector3Mouse(mousePosition);

        if(EditorSettings.BuildBuildingOnMouseActivity_Active)
        {
            var buildingSelected = InstantiateBuildingGo(v3Mouse);
            FirstBuildingOrRoadSelected = buildingSelected;

        }
        else if (EditorSettings.BuildRoadOnMouseActivity_Active)
        {
            var roadSelected = InstantiateRoadGo(v3Mouse);
            if (FirstBuildingOrRoadSelected == null)
            {
                FirstBuildingOrRoadSelected = roadSelected;
            }
            else
            {
                SecondRoadSelected = roadSelected;
            }
        }            
    }

    private Vector3 GetVector3Mouse(Vector2 mousePosition)
    {
        var ray = MainCamera.ScreenPointToRay(mousePosition);
        var hits = Physics.RaycastAll(ray);
        var hitGrassPoint = hits.First(x => x.collider.gameObject.layer == Constants.LAYER_TERRAIN).point;

        hitGrassPoint.y = 0.01f;
        hitGrassPoint.x = (float)Math.Round(hitGrassPoint.x);
        hitGrassPoint.z = (float)Math.Round(hitGrassPoint.z);

        return hitGrassPoint;
    }

    private void FinalizeRoadOrBuilding()
    {
        if (EditorSettings.BuildBuildingOnMouseActivity_Active)
        {
            FinalizeRoadOrBuilding(FirstBuildingOrRoadSelected, isRoad: false);
        }
        else if (EditorSettings.BuildRoadOnMouseActivity_Active)
        {
            if (FirstBuildingOrRoadSelected != null && SecondRoadSelected == null)
            {
                FinalizeRoadOrBuilding(FirstBuildingOrRoadSelected, isRoad: true);
            }
            else if (FirstBuildingOrRoadSelected != null && SecondRoadSelected != null)
            {
                CreateAndFinalizeMultipleRoads();
            }
        }

        FirstBuildingOrRoadSelected = null;
        SecondRoadSelected = null;
    }

    private void CreateAndFinalizeMultipleRoads()
    {
        var allRoadsToFinalize = CreateRoadsBetweenFirstAndSecond();
        foreach(var road in allRoadsToFinalize)
        {
            FinalizeRoadOrBuilding(road, isRoad: true);
        }
    }

    private List<GameObject> CreateRoadsBetweenFirstAndSecond()
    {
        var xLow = (int)Math.Min(FirstBuildingOrRoadSelected.transform.position.x, SecondRoadSelected.transform.position.x);
        var zLow = (int)Math.Min(FirstBuildingOrRoadSelected.transform.position.z, SecondRoadSelected.transform.position.z);

        var xMax = (int)Math.Max(FirstBuildingOrRoadSelected.transform.position.x, SecondRoadSelected.transform.position.x);
        var zMax = (int)Math.Max(FirstBuildingOrRoadSelected.transform.position.z, SecondRoadSelected.transform.position.z);

        var roadsToFinalize = new List<GameObject> { FirstBuildingOrRoadSelected, SecondRoadSelected };

        for (int x = xLow; x <= xMax; x++)
        {
            for (int z = zLow; z <= zMax; z++)
            {
                var newPositionForRoad = new Vector3(x, 0.01f, z);
                if (!FirstBuildingOrRoadSelected.transform.position.IsSameVector3(newPositionForRoad) &&
                    !SecondRoadSelected.transform.position.IsSameVector3(newPositionForRoad))
                {
                    var newRoad = InstantiateRoadGo(newPositionForRoad);
                    roadsToFinalize.Add(newRoad);
                }
            }
        }

        return roadsToFinalize;
    }

    private void FinalizeRoadOrBuilding(GameObject go, bool isRoad)
    {
        var ghost = go.GetComponentInChildren<GhostBuildingBehaviour>();
        ghost.gameObject.SetActive(false);

        var real = GetReal(go);
        real.gameObject.SetActive(true);
        var children = real.GetComponentsInChildren<MonoBehaviour>();
        foreach (var monoBehaviour in children) monoBehaviour.enabled = true;

        var editorBehaviour = go.GetComponent<EditorBuildBehaviour>();
        DestroyImmediate(editorBehaviour);

        var buildingBehaviour = go.GetComponent<BuildingBehaviour>();        
        buildingBehaviour.CurrentBuildStatus = BuildStatus.COMPLETED_BUILDING;
               
        go.name = go.name.Replace("_To_Place", "");

        if (isRoad)
        {
            go.transform.SetParent(NavMeshSurfaceParent.transform);
        }
        else
        {
            buildingBehaviour.BuildingType = EditorSettings.SelectedBuildingType.Value;

            var showResources = real.AddComponent<DisplayBuildingInputOutputHandler>();
            showResources.DisplayProcessingInputOutputPrefab = DisplayProcessingInputOutputPrefab;
            showResources.GoSpawnOffset = GetBuildingPrefabItem(EditorSettings.SelectedBuildingType.Value).DisplayOffset;

            var locOfResource = real.GetComponent<ILocationOfResource>();
            if (locOfResource != null)
            {
                var rangedDisplay = Instantiate(MonoHelper.Instance.RangedDisplayPrefab, ((MonoBehaviour)locOfResource).transform);
                rangedDisplay.MaxRangeForResource = locOfResource.GetMaxRangeForResource();
                rangedDisplay.RangeType = locOfResource.GetRangeTypeToFindResource();
            }
        }

        // anders wordt BuildingType-verandering niet opgeslagen
        EditorUtility.SetDirty(buildingBehaviour);
    }    

    private GameObject GetReal(GameObject roadGo)
    {
        for(var i = 0;i < roadGo.transform.childCount;i++)
        {
            var childGo = roadGo.transform.GetChild(i);
            if(childGo.GetComponent<GhostBuildingBehaviour>() == null && childGo.tag != Constants.TAG_ENTRANCE_EXIT)
            {
                return childGo.gameObject;
            }
        }

        throw new Exception("Alleen maar een ghost??!?!?!");
    }

    private bool ClickOnEditorRoadOrBuilding(Vector2 mousePosition)
    {
        Ray ray = MainCamera.ScreenPointToRay(mousePosition);
        var hits = Physics.RaycastAll(ray);

        foreach (var hit in hits)
        {
            var editorRoad = hit.transform.gameObject.GetComponent<EditorBuildBehaviour>();
            if (editorRoad != null)
            {
                return true;
            }
        }

        return false;
    }
}

