using UnityEngine;
using System.Collections.Generic;
using System;

public class CardSelectProdUiHandler : MonoBehaviour, ICardCarousselDisplay, IProcesOneItemUI, IUiCallingBuilding
{   
    public UiCardBehaviour UiCardBehaviourPrefab;
    [HideInInspector] public List<UiCardBehaviour> UiCardBehaviours;

    public BuildingCategory BuildingCategory; // het type building waar de kaarten voor worden gehaald

    [HideInInspector] public bool CardsAreLoaded;

    public bool ShowRequiredItemsUnderCard = true;

    [HideInInspector] public ICardSelectProdBuilding CallingBuilding;    

    void OnEnable()
    {
        UiCardBehaviours = new List<UiCardBehaviour>();
        var uitSettingsForCard = CallingBuilding.GetBuildingType().GetProductionSettings();

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

    private void OnDisable()
    {
        foreach (var uiCard in UiCardBehaviours)
        {
            Destroy(uiCard.gameObject);
        }
        UiCardBehaviours.Clear();
        CardsAreLoaded = false;
    }

    public void AddAmount(Enum type, int amount)
    {
        var queueBehaviour = CallingBuilding.GetGameObject().GetComponent<QueueForBuildingBehaviour>();
        if (queueBehaviour != null)
        {
            queueBehaviour.AddItemsOnQueue(type, amount);
        }
        else
        {
            CallingBuilding.AddTypes(type, amount);            
        }
    }

    public void DecreaseAmount(Enum type, int amount)
    {
        CallingBuilding.DecreaseTypes(type, amount);
    }
    public int GetCount(Enum type) => CallingBuilding != null ? CallingBuilding.GetCount(type) : 0;

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

    public TypeProcessing GetCurrentItemProcessed() =>  CallingBuilding?.GetCurrentTypeProcessed();
    public float GetBuildTimeInSeconds() => CallingBuilding.GetProductionTime();
    public GameObject GetGameObject() => CallingBuilding?.GetGameObject();
}