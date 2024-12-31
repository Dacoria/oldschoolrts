using UnityEngine;
using System.Collections.Generic;
using System;

public class CardUiHandler : MonoBehaviour, ICardCarousselDisplay, IProcesOneItemUI
{   
    public UiCardBehaviour UiCardBehaviourPrefab;
    [HideInInspector] public List<UiCardBehaviour> UiCardBehaviours;

    public BuildingType BuildingType; // het type building waar de kaarten voor worden gehaald

    [HideInInspector] public bool CardsAreLoaded;

    public bool ShowRequiredItemsUnderCard = true;

    [HideInInspector] public ICardBuilding CallingBuilding;    

    void Start()
    {
        UiCardBehaviours = new List<UiCardBehaviour>();

        var uitSettingsForCard = BuildingType.GetProductionSettings();

        foreach (var uiSetting in uitSettingsForCard)
        {
            var uiCardGo = Instantiate(UiCardBehaviourPrefab, transform);
            uiCardGo.Image.sprite = uiSetting.GetIcon();

            var enumType = uiSetting.GetType();
            var enumString = enumType.ToString();

            uiCardGo.TitleText.text = Char.ToLowerInvariant(enumString[0]).ToString().ToUpper() + enumString.ToLower().Substring(1);
            UiCardBehaviours.Add(uiCardGo);

            uiCardGo.Type = enumType;
            uiCardGo.CardUiHandler = this;
        }

        CardsAreLoaded = true;

        if(UiCardBehaviours.Count > 0)
        {
            ShowStatsOfUnitCard(UiCardBehaviours[0]);
        }
    }

    public void OnEnable()
    {
        if (UiCardBehaviours.Count > 0)
        {
            ShowStatsOfUnitCard(UiCardBehaviours[0]);
        }
    }

    public void AddAmount(Enum type, int amount)
    {
        var queueBehaviour = CallingBuilding.GetQueueForBuildingBehaviour();

        var queuelimit = 14; // TODO: Nette plek???? lijkt niet hier
        var amountToAdd = queueBehaviour == null ? amount : Math.Min(queuelimit - queueBehaviour.QueueItems.Count, amount);

        for (int i = 0; i < amountToAdd; i++)
        {            
            if (queueBehaviour != null)
            {
                queueBehaviour.AddItemOnQueue(type);
            }
            else
            {
                CallingBuilding.AddItem(type);
            }                                  
        }
    }

    public void DecreaseAmount(Enum type, int amount)
    {
        for (int i = 0;i < amount;i++)
        {
            CallingBuilding.DecreaseItem(type);
        }        
    }
    public int GetCount(Enum type)
    {
        return CallingBuilding != null ? CallingBuilding.GetCount(type) : 0;
    }

    public void ClickOnCardLeftClick(UiCardBehaviour displayUiCardBehaviour)
    {
        ShowStatsOfUnitCard(displayUiCardBehaviour);
    }

    private void ShowStatsOfUnitCard(UiCardBehaviour displayUiCardBehaviour)
    {
        var selectedDisplayCard = transform.parent.GetComponentInChildren<SelectedDisplayCardScript>();
        if (selectedDisplayCard != null)
        {
            selectedDisplayCard.SelectedDisplayUiCard = displayUiCardBehaviour;
        }
    }

    public int GetCount() => UiCardBehaviours.Count;

    bool ICardCarousselDisplay.CardsAreLoaded() => CardsAreLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        UiCardBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public UIItemProcessing GetCurrentItemProcessed() =>  CallingBuilding?.GetCurrentItemProcessed();
    public float GetBuildTimeInSeconds(Enum type) => CallingBuilding.GetProductionTime(type);
}