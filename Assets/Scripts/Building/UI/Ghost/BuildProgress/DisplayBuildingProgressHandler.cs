using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBuildingProgressHandler : BaseAEMonoCI
{
    public DisplayBuildingProgress DisplayBuildingProgressPrefab;
    [HideInInspector] public GameObject DisplayBuildingProgressGo;

    [ComponentInject] private BuildingBehaviour BuildingBehaviourScript;

    public Vector3 GoSpawnOffset = new Vector3(0, 0, 0);
    public Vector3 GoSpawnScaleOffset = new Vector3(1, 1, 1);

    private bool isScriptActive = false;
    private GameObject InputDisplayGoPrefab;

    public void Start()
    {
        if (BuildingBehaviourScript == null)
        {
            throw new Exception("altijd BuildingBehaviour vereist voor GenerateBuildingProgressDisplayResources");
        }

        if(DisplayBuildingProgressGo == null)
        {
            Destroy(this);
        }
    }

    private int frameCounter;
    private int framesTillSlowUpdate = 20;

    // niet via mono-extension --> deze class extend al AE-base
    void Update()
    {
        if(!isScriptActive)
        {
            if(BuildingBehaviourScript.CurrentBuildStatus == BuildStatus.NEEDS_PREPARE)
            {
                StartCreatingBuildProgressDisplay(); // zet ook isScriptActive op true
            }            
        }
        else
        {
            if (BuildingBehaviourScript.CurrentBuildStatus == BuildStatus.COMPLETED_BUILDING)
            {
                // DESTROY
                Destroy(DisplayBuildingProgressGo);
                Destroy(this);
            }                     
        }

        // niet via mono-extension --> deze class extend al AE-base
        frameCounter++;
        if (frameCounter >= framesTillSlowUpdate)
        {
            frameCounter = 0;
            SlowUpdate();
        }
    } 

    private void SlowUpdate()
    {
        if (isScriptActive && KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active)
        {
            UpdateInputText();
        }
    }

    protected override void OnKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        if (keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleBuildingProgressDisplay)
        {
            UpdateEnabledStatusOfDisplayObjects();
        }
    }

    private void UpdateEnabledStatusOfDisplayObjects()
    {
        for (var i = 0; i < DisplayBuildingProgressGo.transform.childCount; i++)
        {
            var child = DisplayBuildingProgressGo.transform.GetChild(i);
            child.gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active);
        }
    }

    private void StartCreatingBuildProgressDisplay()
    {
        DisplayBuildingProgressGo = Instantiate(DisplayBuildingProgressPrefab.gameObject, transform);
        DisplayBuildingProgressGo.transform.position = transform.position + GoSpawnOffset;
        DisplayBuildingProgressGo.transform.localScale = DisplayBuildingProgressGo.transform.localScale.MultiplyVector(GoSpawnScaleOffset);

        InputDisplayGoPrefab = DisplayBuildingProgressGo.transform.Find("InputPrefab").gameObject;
        var buildPrefab = DisplayBuildingProgressGo.transform.Find("BuildPrefab").gameObject;       

        buildPrefab.SetActive(true);
        InitiateItemsDisplay();
        UpdateEnabledStatusOfDisplayObjects();

        Destroy(InputDisplayGoPrefab);

        isScriptActive = true;
    }

    public void FixDisplayForItemtype(List<TextMeshItem> ListToAddResult, ItemType itemType, GameObject displayGo)
    {
        displayGo.SetActive(true);
        var textMesh = displayGo.GetComponentInChildren<TextMeshPro>();
        ListToAddResult.Add(new TextMeshItem() { ItemType = itemType, TextMesh = textMesh });

        var rendererResourceImage = displayGo.GetComponentsInChildren<Renderer>().FirstOrDefault(x => x.name.StartsWith("ResourceImage"));
        if (rendererResourceImage != null)
        {
            var prefabsForItemtype = ResourcePrefabs.Get().Single(x => x.ItemType == itemType);
            rendererResourceImage.material.mainTexture = prefabsForItemtype.Icon.texture;
        }
    }

    private List<TextMeshItem> InputTextMeshItems;    

    private void InitiateItemsDisplay()
    {
        InputTextMeshItems = new List<TextMeshItem>();
        for (var i = 0; i < BuildingBehaviourScript.RequiredItems.Count; i ++)
        {
            var inputDisplayGo = Instantiate(InputDisplayGoPrefab, DisplayBuildingProgressGo.transform);

            var extraYDistance = ((BuildingBehaviourScript.RequiredItems.Count - 1) * 0.6f) - (i * 0.6f);
            inputDisplayGo.transform.position = new Vector3(inputDisplayGo.transform.position.x, inputDisplayGo.transform.position.y + extraYDistance, inputDisplayGo.transform.position.z);

            FixDisplayForItemtype(InputTextMeshItems, BuildingBehaviourScript.RequiredItems[i].ItemType, inputDisplayGo);
        }
    }    

    private void UpdateInputText()
    {
        foreach (var itemsRequiredForBuilding in BuildingBehaviourScript.RequiredItems)
        {
            var textMesh = InputTextMeshItems.Single(x => x.ItemType == itemsRequiredForBuilding.ItemType).TextMesh;
            var itemOfTypeAlreadyDelivered = BuildingBehaviourScript.GetItemCountDeliveredForBuilding(itemsRequiredForBuilding.ItemType);

            textMesh.text = $"{itemOfTypeAlreadyDelivered}/{itemsRequiredForBuilding.Amount}";
        }
    }    

    public class TextMeshItem
    {
        public TextMeshPro TextMesh;
        public ItemType ItemType;
    }
}