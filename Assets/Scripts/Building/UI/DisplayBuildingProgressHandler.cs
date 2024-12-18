using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayBuildingProgressHandler : BaseAEMonoCI
{
    public GameObject BuildingProgressDisplayPrefab;
    [HideInInspector] public GameObject BuildingProgressDisplayGo;

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
                Destroy(BuildingProgressDisplayGo);
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
        for (var i = 0; i < BuildingProgressDisplayGo.transform.childCount; i++)
        {
            var child = BuildingProgressDisplayGo.transform.GetChild(i);
            child.gameObject.SetActive(KeyCodeStatusSettings.ToggleBuildingProgressDisplay_Active);
        }
    }

    private void StartCreatingBuildProgressDisplay()
    {
        BuildingProgressDisplayGo = Instantiate(BuildingProgressDisplayPrefab, transform);
        BuildingProgressDisplayGo.transform.position = transform.position + GoSpawnOffset;
        BuildingProgressDisplayGo.transform.localScale = BuildingProgressDisplayGo.transform.localScale.MultiplyVector(GoSpawnScaleOffset);

        InputDisplayGoPrefab = BuildingProgressDisplayGo.transform.Find("InputPrefab").gameObject;
        var buildPrefab = BuildingProgressDisplayGo.transform.Find("BuildPrefab").gameObject;       

        buildPrefab.SetActive(true);
        InitiateItemsDisplay();

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
            var prefabsForItemtype = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == itemType);
            rendererResourceImage.material.mainTexture = prefabsForItemtype.Icon.texture;
        }
    }

    private List<TextMeshItem> InputTextMeshItems;    

    private void InitiateItemsDisplay()
    {
        InputTextMeshItems = new List<TextMeshItem>();
        for (var i = 0; i < BuildingBehaviourScript.RequiredItems.Length; i ++)
        {
            var inputDisplayGo = Instantiate(InputDisplayGoPrefab, BuildingProgressDisplayGo.transform);

            var extraYDistance = ((BuildingBehaviourScript.RequiredItems.Length - 1) * 0.6f) - (i * 0.6f);
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

            textMesh.text = itemOfTypeAlreadyDelivered +
                "/" +
                itemsRequiredForBuilding.Amount;
        }
    }    

    public class TextMeshItem
    {
        public TextMeshPro TextMesh;
        public ItemType ItemType;
    }
}