using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class DisplayBuildingInputOutputHandler : BaseAEMonoCI
{
    public DisplayProcessingInputOutput DisplayProcessingInputOutputPrefab;

    private GameObject processingDisplayGo;

    [ComponentInject(Required.OPTIONAL)] private RefillBehaviour refillBehaviour;

    [ComponentInject] private BuildingBehaviour buildingBehaviour;
    private BuildingCategory buildingCategory => buildingBehaviour.BuildingType.GetCategory();

    [ComponentInject(Required.OPTIONAL)] private HandleProduceResourceOrderBehaviour produceOrderBehaviour;

    private bool ShowProdPerCycleInput() =>
        buildingCategory == BuildingCategory.Mine ||
        buildingCategory == BuildingCategory.OneProductOverTime ||
        buildingCategory == BuildingCategory.Manual;

    private bool ShowInput() =>
        buildingCategory == BuildingCategory.Mine ||
        buildingCategory == BuildingCategory.OneProductOverTime ||
        buildingCategory == BuildingCategory.Manual ||
        buildingCategory == BuildingCategory.SelectProductsOverTime;

    private bool ShowOutput() =>
        produceOrderBehaviour != null && (
            buildingCategory == BuildingCategory.Mine ||
            buildingCategory == BuildingCategory.OneProductOverTime ||
            buildingCategory == BuildingCategory.Manual
        );

    private bool ShowGears() =>
        produceOrderBehaviour != null && (
            buildingCategory == BuildingCategory.Mine ||
            buildingCategory == BuildingCategory.OneProductOverTime
        );


    public Vector3 GoSpawnOffset = new Vector3(0, 0, 0);
    public Vector3 GoSpawnScaleOffset = new Vector3(1, 1, 1);
    
    private GameObject InputDisplayPrefabGo;
    private GameObject OutputDisplayPrefabGo;

    private bool scriptIsLoaded;

    protected override void Awake()
    {
        // SKIP CI -> doe later
    }

    IEnumerator Start()
    {
        this.ComponentInject();

        // altijd wachten tot alles klaar is
        yield return Wait4Seconds.Get(1);

        this.ComponentInject(); // later om zeker te zijn dat de component er zijn (soms gegenereert uit ander script)

        processingDisplayGo = Instantiate(DisplayProcessingInputOutputPrefab.gameObject, transform);
        processingDisplayGo.transform.position = transform.position + GoSpawnOffset;
        processingDisplayGo.transform.localScale = processingDisplayGo.transform.localScale.MultiplyVector(GoSpawnScaleOffset);

        InputDisplayPrefabGo = processingDisplayGo.transform.Find("InputPrefab").gameObject;
        OutputDisplayPrefabGo = processingDisplayGo.transform.Find("OutputPrefab").gameObject;

        var gearsDisplayGo = processingDisplayGo.transform.Find("GearPrefab").gameObject;

        if (ShowInput())
        {
            InitiateInputDisplayReqScript();
        }
        if (ShowOutput())
        {
            InitiateOutputDisplayProdScript();
            if(ShowGears())
            {
                gearsDisplayGo.SetActive(true); // gears draaien als het prod script produceert
            }
            else
            {
                Destroy(gearsDisplayGo);
            }            
        }
        else
        {
            Destroy(gearsDisplayGo);
        }

        Destroy(InputDisplayPrefabGo);
        Destroy(OutputDisplayPrefabGo);

        UpdateEnabledStatusOfDisplayObjects(KeyCodeStatusSettings.ToggleInputOutputDisplay_Active);
        
        scriptIsLoaded = true;
    }

    private List<TextMeshItem> InputTextMeshItems;
    private List<TextMeshItem> OutputTextMeshItems;

    private void InitiateInputDisplayReqScript()
    {
        InputTextMeshItems = new List<TextMeshItem>();
        for (int i = 0;i < refillBehaviour.GetItemsToRefill().Count; i ++)
        {
            var inputDisplayGo = Instantiate(InputDisplayPrefabGo, processingDisplayGo.transform);
            var extraYDistance = ((refillBehaviour.GetItemsToRefill().Count - 1) * 0.6f) - (i * 0.6f);
            inputDisplayGo.transform.position = new Vector3(inputDisplayGo.transform.position.x, inputDisplayGo.transform.position.y + extraYDistance, inputDisplayGo.transform.position.z);
            FixInputOutputDisplayForItemtype(InputTextMeshItems, refillBehaviour.GetItemsToRefill()[i].ItemType, inputDisplayGo);
        }        
    }  

    private void InitiateOutputDisplayProdScript()
    {
        OutputTextMeshItems = new List<TextMeshItem>();
        var itemsToProduce = buildingBehaviour.BuildingType.GetItemProduceSettings().First();
        for (int i = 0; i < itemsToProduce.ItemsToProduce.Count(); i++)
        {
            var outputDisplayGo = Instantiate(OutputDisplayPrefabGo, processingDisplayGo.transform);
            var extraYDistance = ((itemsToProduce.ItemsToProduce.Count() - 1) * 0.6f) - (i * 0.6f);
            outputDisplayGo.transform.position = new Vector3(outputDisplayGo.transform.position.x, outputDisplayGo.transform.position.y + extraYDistance, outputDisplayGo.transform.position.z);
            FixInputOutputDisplayForItemtype(OutputTextMeshItems, itemsToProduce.ItemsToProduce[i].ItemType, outputDisplayGo);
        }               
    }

    public void FixInputOutputDisplayForItemtype(List<TextMeshItem> ListToAddResult, ItemType itemType, GameObject displayGo)
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

    private int framesTillSlowUpdate = 10;
    private int frameCounter;

    // niet via mono-extension --> deze class extend al AE-base
    private void Update()
    {
        frameCounter++;
        if (frameCounter >= framesTillSlowUpdate)
        {
            frameCounter = 0;
            SlowUpdate();
        }
    }

    private void SlowUpdate()
    {
        if (scriptIsLoaded && processingDisplayGo.transform.childCount > 0 && processingDisplayGo.transform.GetChild(0).gameObject.activeSelf)
        {
            UpdateInputText();
            UpdateOutputText();
        }
    }

    protected override void OnKeyCodeAction(KeyCodeAction keyCodeAction)
    {
        if (keyCodeAction.KeyCodeActionType == KeyCodeActionType.ToggleInputOutputDisplay)
        {
            UpdateEnabledStatusOfDisplayObjects(KeyCodeStatusSettings.ToggleInputOutputDisplay_Active);
        }
    }

    public void UpdateEnabledStatusOfDisplayObjects(bool isActive)
    {
        if (processingDisplayGo?.transform?.childCount != null)
        {
            processingDisplayGo.SetDirectChildrenSetActive(isActive);
        }
    }

    private void UpdateOutputText()
    {
        if (ShowOutput())
        {
            var itemsToProduce = buildingBehaviour.BuildingType.GetItemProduceSettings().First();
            foreach (var itemToProduce in itemsToProduce.ItemsToProduce)
            {
                var textMesh = OutputTextMeshItems.Single(x => x.ItemType == itemToProduce.ItemType).TextMesh;
                var orderCount = produceOrderBehaviour.OutputOrders.Count(x => x.ItemType == itemToProduce.ItemType);
                textMesh.text = $"{orderCount}/{itemToProduce.MaxBuffer} ({itemToProduce.ProducedPerProdCycle}x)";
            }
        }        
    }

    private void UpdateInputText()
    {
        if (refillBehaviour != null)
        {
            foreach (var itemConsumed in refillBehaviour.GetItemsToRefill())
            {
                var textMesh = InputTextMeshItems.Single(x => x.ItemType == itemConsumed.ItemType).TextMesh;
                var itemInStockpile = refillBehaviour.StockpileOfItemsRequired.Single(x => x.ItemType == itemConsumed.ItemType);
                var maxBuffer = refillBehaviour.GetItemCountToRefill(itemConsumed.ItemType, 0, 0);
                var hasMaxBuffer = true;

                textMesh.text = itemInStockpile.Amount.ToString();
                if (hasMaxBuffer)
                {
                    textMesh.text += $"/{maxBuffer}";
                    if (ShowProdPerCycleInput())
                    {
                        // alleen dit showen bij de '1 ding produceren per keer' gebouwen (dus niet barracks, wel waterwel etc)
                        var consumeItemsFirstProduction = buildingBehaviour.BuildingType.GetItemProduceSettings().First().ItemsConsumedToProduce; 
                        textMesh.text += $" ({consumeItemsFirstProduction.First(x => x.ItemType == itemConsumed.ItemType).Amount}x)";
                    }
                }
            }                   
        }   
    }

    public class TextMeshItem
    {
        public TextMeshPro TextMesh;
        public ItemType ItemType;
    }    
}