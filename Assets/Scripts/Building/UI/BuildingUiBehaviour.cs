using System;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUiBehaviour : MonoBehaviour, ICardCarousselDisplay
{
    public BuildingUiWrapperBehaviour BuildingUiWrapperBehaviourPrefab;
    public List<BuildingUiWrapperBehaviour> BuildingUiWrapperBehaviours;
    private BuildBuildingsByUser BuildBuildingsByUser;
    private UiManager UiManager;

    private bool CardsLoaded;


    public void Awake()
    {
        this.ComponentInject();
    }

    private void Start()
    {
        BuildingUiWrapperBehaviours = new List<BuildingUiWrapperBehaviour>();
        foreach (var BuildingSettings in GameManager.Instance.BuildingPrefabItems.OrderBy(x => (int)x.BuildingType))
        {
            var buildingUiWrapper = Instantiate(BuildingUiWrapperBehaviourPrefab, transform);
            buildingUiWrapper.GetComponentInChildren<Image>().sprite = BuildingSettings.Icon;

            buildingUiWrapper.BuildingType = BuildingSettings.BuildingType;
            buildingUiWrapper.BuildingUiBehaviour = this;
            buildingUiWrapper.Text.text = Char.ToLowerInvariant(buildingUiWrapper.BuildingType.ToString()[0]).ToString().ToUpper() + buildingUiWrapper.BuildingType.ToString().ToLower() .Substring(1);
            BuildingUiWrapperBehaviours.Add(buildingUiWrapper);
        }

        BuildBuildingsByUser = FindObjectOfType<BuildBuildingsByUser>();
        if(BuildBuildingsByUser == null) { throw new Exception("Geen BuildBuildingsByUser in Scene!"); }
        UiManager = FindObjectOfType<UiManager>();
        if (BuildBuildingsByUser == null) { throw new Exception("Geen UiManager in Scene!"); }

        CardsLoaded = true;
    }

    public void BuildBuilding(BuildingType buildingType)
    {
        var buildingItem = GameManager.Instance.BuildingPrefabItems.Single(x => x.BuildingType == buildingType);
        BuildBuildingsByUser.BuildGeneric(buildingItem.BuildingPrefab);
        UiManager.ActivateUI(false);
    }

    public int GetCount() => BuildingUiWrapperBehaviours.Count;

    public bool CardsAreLoaded() => CardsLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        BuildingUiWrapperBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public int GetIndexFirstEnabledCard()
    {
        for (int i = 0; i < BuildingUiWrapperBehaviours.Count; i++)
        {
            var card = BuildingUiWrapperBehaviours[i];
            if (card.gameObject.activeSelf)
            {
                return i;
            }
        }

        return -1;
    }
}