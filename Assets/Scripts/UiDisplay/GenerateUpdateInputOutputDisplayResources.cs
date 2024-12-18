using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

public class GenerateUpdateInputOutputDisplayResources : MonoBehaviourSlowUpdateFramesCI
{    
    public GameObject ProcessingDisplayPrefab;

    private GameObject ProcessingDisplayGo;

    [ComponentInject(Required.OPTIONAL)] private RefillBehaviour RefillBehaviour;
    [ComponentInject(Required.OPTIONAL)] private ProduceResourceOrderBehaviour ProduceResourceBehaviourScript;

    private BarracksBehaviour BarracksBehaviour;
    private QueueForBuildingBehaviour QueueForBuildingBehaviour;


    public Vector3 GoSpawnOffset = new Vector3(0, 0, 0);
    public Vector3 GoSpawnScaleOffset = new Vector3(1, 1, 1);
    
    private GameObject InputDisplayGo;
    private GameObject OutputDisplayGo;
    private GameObject ProgressCircleGo;
    private GameObject ProgressCircleGoRunning;

    private bool scriptIsLoaded;


    IEnumerator Start()
    {
        // altijd wachten tot alles klaar is
        yield return MonoHelper.Instance.GetCachedWaitForSeconds(1);

        this.ComponentInject(); // later om zeker te zijn dat de component er zijn (soms gegenereert uit ander script)
        BarracksBehaviour = GetComponent<BarracksBehaviour>(); // eenmalig
        QueueForBuildingBehaviour = GetComponent<QueueForBuildingBehaviour>();

        ProcessingDisplayGo = Instantiate(ProcessingDisplayPrefab, transform);
        ProcessingDisplayGo.transform.position = transform.position + GoSpawnOffset;
        ProcessingDisplayGo.transform.localScale = ProcessingDisplayGo.transform.localScale.MultiplyVector(GoSpawnScaleOffset);

        InputDisplayGo = ProcessingDisplayGo.transform.Find("InputPrefab").gameObject;
        OutputDisplayGo = ProcessingDisplayGo.transform.Find("OutputPrefab").gameObject;
        ProgressCircleGo = ProcessingDisplayGo.transform.Find("ProgressCirclePrefab").gameObject;

        var gearsDisplayGo = ProcessingDisplayGo.transform.Find("GearPrefab").gameObject;

        if (RefillBehaviour != null && BarracksBehaviour == null)
        {
            InitiateInputDisplayReqScript();
        }
        if (ProduceResourceBehaviourScript != null)
        {
            InitiateOutputDisplayProdScript();
            gearsDisplayGo.SetActive(ProduceResourceBehaviourScript.IsProducingResourcesOverTime()); // gears draaien als het prod script produceert
        }
        if (QueueForBuildingBehaviour != null)
        {
            ProgressCircleGoRunning = ProgressCircleGo.transform.Find("fragment").gameObject;
        }

        scriptIsLoaded = true;
    }

    private List<TextMeshItem> InputTextMeshItems;
    private List<TextMeshItem> OutputTextMeshItems;

    private void InitiateInputDisplayReqScript()
    {
        InputTextMeshItems = new List<TextMeshItem>();
        for (int i = 0;i < RefillBehaviour.GetItemsToRefill().Count; i ++)
        {
            var inputDisplayGo = Instantiate(InputDisplayGo, ProcessingDisplayGo.transform);
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
            var outputDisplayGo = Instantiate(OutputDisplayGo, ProcessingDisplayGo.transform);
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

    protected override int FramesTillSlowUpdate => 10;
    protected override void SlowUpdate()
    {
        if (scriptIsLoaded)
        {
            UpdateInputText();
            UpdateOutputText();
            UpdateProgressCircleText();
        }
    }

    private void UpdateProgressCircleText()
    {
        if(QueueForBuildingBehaviour != null)
        {
            ProgressCircleGo.SetActive(QueueForBuildingBehaviour.QueueItems.Any());
            ProgressCircleGoRunning.SetActive(QueueForBuildingBehaviour.GetCurrentItemProcessed() != null);
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