using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class FoodConsumptionBehaviour : BaseAEMonoCI
{
    public float FoodSatisfactionPercentage = 0.6f;
    public float FoodDeclinePercPerSecond = 0.01f;
    public float PercLimitForFoodRefill = 0.2f;

    public FoodConsumption FoodConsumption;
    public FoodConsumptionStatus Status => FoodConsumption != null ? FoodConsumption.FoodConsumptionStatus : FoodConsumptionStatus.NONE; // alleen voor gemak

    [ComponentInject] private NavMeshAgent NavMeshAgent;
    private List<IHasStopped> hasStoppedScripts;

    public bool UseTavernBubble = true;
    private GameObject GoToTavernBubble;

    void Start()
    {
        FoodConsumption = new FoodConsumption(FoodSatisfactionPercentage, FoodDeclinePercPerSecond, PercLimitForFoodRefill);
        hasStoppedScripts = GetComponents<IHasStopped>().ToList();
        StartCoroutine(ProcessFoodDecline());
    }

    protected override void OnFoodStatusHasChanged(FoodConsumption foodConsumption, FoodConsumptionStatus previousStatus)
    {
        if(foodConsumption == FoodConsumption)
        {        
            switch (foodConsumption.FoodConsumptionStatus)
            {
                case FoodConsumptionStatus.IS_REFILLING:
                    HideShowGo(this.gameObject, false);
                    break;
                case FoodConsumptionStatus.REFILL_SUCCESS:
                    HideShowGo(this.gameObject, true);                    
                    break;
            }

            // tavern bubble
            if (UseTavernBubble)
            {
                if (foodConsumption.FoodConsumptionStatus == FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT)
                {
                    InitiateGoToTavernBubble();
                }
                else if (previousStatus == FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT)
                {
                    FinishedGoToTavernBubble();
                }
            }
        }
    }   

    private void Update()
    {
        if (Status == FoodConsumptionStatus.NEEDS_REFILL)
        {
            TryStartRefillProcess();
        }

        else if (Status == FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT && 
            NavMeshAgent.StoppedAtDestination(0.7f))
        {
            ReachedRefillPoint();
            NavMeshAgent.isStopped = true;
        }
        FoodSatisfactionPercentage = FoodConsumption.FoodSatisfactionPercentage; //makkelijker debuggen....
    }

    private void TryStartRefillProcess()
    {        
        if(hasStoppedScripts.All(x => x.HasStoppedWithLogic()))
        {
            if(FoodConsumption.TrySetTavernToGetFood())
            {
                NavMeshAgent.destination = FoodConsumption.TavernTargetedForFoodRefill.gameObject.EntranceExit();
                NavMeshAgent.isStopped = false;
                FoodConsumption.FoodConsumptionStatus = FoodConsumptionStatus.GO_TOWARDS_REFILLL_POINT;
            }
        }
    }

    private IEnumerator ProcessFoodDecline()
    {
        yield return Wait4Seconds.Get(1);
        if (FoodConsumptionSettings.ToggleUseFoodConsumption_Active)
        {
            FoodConsumption.ConsumeFood();
        }        
        StartCoroutine(ProcessFoodDecline());
    }

    private void HideShowGo(GameObject go, bool isEnabled)
    {
        if (go.GetComponent<Renderer>())
        {
            go.GetComponent<Renderer>().enabled = isEnabled;
        }
        foreach (var render in go.GetComponentsInChildren<Renderer>())
        {
            render.enabled = isEnabled;
        }
    }

    private void InitiateGoToTavernBubble()
    {
        GoToTavernBubble = Instantiate(Load.GoMap["GoToTavernBubble"], this.transform, false);
        GoToTavernBubble.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        GoToTavernBubble.transform.localPosition = new Vector3(0, 2.4f, 0); // net voor de borst -> voor nu hardcoded, allemaal cubes
    }

    private void FinishedGoToTavernBubble()
    {
        if (GoToTavernBubble != null)
        {
            Destroy(GoToTavernBubble);
        }
    }

    public void ReachedRefillPoint()
    {
        AE.ReachedFoodRefillingPoint?.Invoke(FoodConsumption.TavernTargetedForFoodRefill, this);
        FoodConsumption.TavernTargetedForFoodRefill = null;
    }
}