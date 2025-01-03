using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class BuildingUiBehaviour : MonoBehaviourCI, ICardCarousselDisplay
{
    public BuildingUiWrapperBehaviour BuildingUiWrapperBehaviourPrefab;
    public List<BuildingUiWrapperBehaviour> BuildingUiWrapperBehaviours;
    private BuildBuildingsByUser BuildBuildingsByUser;
    private UiManager UiManager;

    private bool CardsLoaded;

    private void Start()
    {
        BuildingUiWrapperBehaviours = new List<BuildingUiWrapperBehaviour>();
        foreach (var BuildingSettings in BuildingPrefabs.Get().OrderBy(x => (int)x.BuildingType))
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
        var buildingItem = BuildingPrefabs.Get().Single(x => x.BuildingType == buildingType);
        BuildBuildingsByUser.BuildGeneric(buildingItem.BuildingPrefab);
        UiManager.DisableEntireCanvas();
    }

    public int GetCount() => BuildingUiWrapperBehaviours.Count;

    public bool CardsAreLoaded() => CardsLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        BuildingUiWrapperBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }
}