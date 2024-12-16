using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ShowRequiredResourcesForBuildingBehaviour : MonoBehaviour
{
    public ImageTextBehaviour ReqResourcesPrefab;

    [ComponentInject]
    private BuildingUiWrapperBehaviour BuildingUiWrapperBehaviour;

    void Awake()
    {
        this.ComponentInject();
    }


    void Start()
    {        
        var buildingTypeSettings = GameManager.Instance.BuildingPrefabItems.Single(x => x.BuildingType == BuildingUiWrapperBehaviour.BuildingType);
        var itemsToBuildBuilding = buildingTypeSettings.BuildingPrefab.GetComponent<BuildingBehaviour>();
        if(itemsToBuildBuilding != null)
        {
            foreach(var reqItemToBuild in itemsToBuildBuilding.RequiredItems)
            {
                var reqItemCard = Instantiate(ReqResourcesPrefab, transform);
                reqItemCard.Text.text = reqItemToBuild.Amount.ToString();

                var itemSetting = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == reqItemToBuild.ItemType);
                reqItemCard.Image.sprite = itemSetting.Icon;

                reqItemCard.ItemType = reqItemToBuild.ItemType;
            }
        }

    }
}
