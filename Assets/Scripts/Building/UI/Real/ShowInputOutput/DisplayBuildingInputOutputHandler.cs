using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class DisplayBuildingInputOutputHandler : BaseAEMonoCI
{    
    public DisplayProcessingInputOutput DisplayProcessingInputOutputPrefab;

    private GameObject ProcessingDisplayGo;

    [ComponentInject(Required.OPTIONAL)] private RefillBehaviour RefillBehaviour;
    [ComponentInject(Required.OPTIONAL)] private ProduceResourceOrderBehaviour ProduceResourceBehaviourScript;

    private BarracksBehaviour BarracksBehaviour;
    private QueueForBuildingBehaviour QueueForBuildingBehaviour;


    public Vector3 GoSpawnOffset = new Vector3(0, 0, 0);
    public Vector3 GoSpawnScaleOffset = new Vector3(1, 1, 1);
    
    private GameObject InputDisplayPrefabGo;
    private GameObject OutputDisplayPrefabGo;

    private bool scriptIsLoaded;
    
    IEnumerator Start()
    {
        // altijd wachten tot alles klaar is
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(1);

        this.ComponentInject(); // later om zeker te zijn dat de component er zijn (soms gegenereert uit ander script)
        BarracksBehaviour = GetComponent<BarracksBehaviour>(); // eenmalig
        QueueForBuildingBehaviour = GetComponent<QueueForBuildingBehaviour>();

        ProcessingDisplayGo = Instantiate(DisplayProcessingInputOutputPrefab.gameObject, transform);
        ProcessingDisplayGo.transform.position = transform.position + GoSpawnOffset;
        ProcessingDisplayGo.transform.localScale = ProcessingDisplayGo.transform.localScale.MultiplyVector(GoSpawnScaleOffset);

        InputDisplayPrefabGo = ProcessingDisplayGo.transform.Find("InputPrefab").gameObject;
        OutputDisplayPrefabGo = ProcessingDisplayGo.transform.Find("OutputPrefab").gameObject;

        var gearsDisplayGo = ProcessingDisplayGo.transform.Find("GearPrefab").gameObject;

        if (RefillBehaviour != null && BarracksBehaviour == null)
        {
            InitiateInputDisplayReqScript();
        }
        if (ProduceResourceBehaviourScript != null)
        {
            InitiateOutputDisplayProdScript();
            if(ProduceResourceBehaviourScript.IsProducingResourcesOverTime())
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
        for (int i = 0;i < RefillBehaviour.GetItemsToRefill().Count; i ++)
        {
            var inputDisplayGo = Instantiate(InputDisplayPrefabGo, ProcessingDisplayGo.transform);
            var extraYDistance = ((RefillBehaviour.GetItemsToRefill().Count - 1) * 0.6f) - (i * 0.6f);
            inputDisplayGo.transform.position = new Vector3(inputDisplayGo.transform.position.x, inputDisplayGo.transform.position.y + extraYDistance, inputDisplayGo.transform.position.z);
            FixInputOutputDisplayForItemtype(InputTextMeshItems, RefillBehaviour.GetItemsToRefill()[i].ItemType, inputDisplayGo);
        }        
    }  

    private void InitiateOutputDisplayProdScript()
    {
        OutputTextMeshItems = new List<TextMeshItem>();
        for (int i = 0; i < ProduceResourceBehaviourScript.ResourcesToProduce.GetAvailableItemsToProduce().Count(); i++)
        {
            var outputDisplayGo = Instantiate(OutputDisplayPrefabGo, ProcessingDisplayGo.transform);
            var extraYDistance = ((ProduceResourceBehaviourScript.ResourcesToProduce.GetAvailableItemsToProduce().Count() - 1) * 0.6f) - (i * 0.6f);
            outputDisplayGo.transform.position = new Vector3(outputDisplayGo.transform.position.x, outputDisplayGo.transform.position.y + extraYDistance, outputDisplayGo.transform.position.z);
            FixInputOutputDisplayForItemtype(OutputTextMeshItems, ProduceResourceBehaviourScript.ResourcesToProduce.GetAvailableItemsToProduce()[i].ItemType, outputDisplayGo);
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
            var prefabsForItemtype = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == itemType);
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
        if (scriptIsLoaded && KeyCodeStatusSettings.ToggleInputOutputDisplay_Active)
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
        if (ProcessingDisplayGo?.transform?.childCount != null)
        {
            for (var i = 0; i < ProcessingDisplayGo.transform.childCount; i++)
            {
                var child = ProcessingDisplayGo.transform.GetChild(i);
                child.gameObject.SetActive(isActive);
            }
        }
    }

    private void UpdateOutputText()
    {
        if (ProduceResourceBehaviourScript != null)
        {
            foreach (var itemToProduce in ProduceResourceBehaviourScript.ResourcesToProduce.GetAvailableItemsToProduce())
            {
                var textMesh = OutputTextMeshItems.Single(x => x.ItemType == itemToProduce.ItemType).TextMesh;

                textMesh.text = ProduceResourceBehaviourScript.OutputOrders.Count(x => x.ItemType == itemToProduce.ItemType) + 
                    "/" + 
                    itemToProduce.MaxBuffer + 
                    " (" + itemToProduce.ProducedPerProdCycle + "x)";
            }
        }        
    }

    private void UpdateInputText()
    {
        if (RefillBehaviour != null && BarracksBehaviour == null)
        {
            foreach (var itemConsumed in RefillBehaviour.GetItemsToRefill())
            {
                var textMesh = InputTextMeshItems.Single(x => x.ItemType == itemConsumed.ItemType).TextMesh;
                var itemInStockpile = RefillBehaviour.StockpileOfItemsRequired.Single(x => x.ItemType == itemConsumed.ItemType);
                var maxBuffer = RefillBehaviour.GetItemCountToRefill(itemConsumed.ItemType, 0, 0);
                var hasMaxBuffer = !RefillBehaviour.RefillItems.AlwaysRefillItemsIgnoreBuffer();

                textMesh.text = itemInStockpile.Amount.ToString();
                if(hasMaxBuffer)
                    textMesh.text += 
                    "/" +
                    maxBuffer +
                    " (" + itemConsumed.Amount + "x)";
            }
        }   
    }

    public class TextMeshItem
    {
        public TextMeshPro TextMesh;
        public ItemType ItemType;
    }    
}