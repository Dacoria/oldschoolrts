using System.Linq;

public class ShowRequiredResourcesForBuildingBehaviour : MonoBehaviourCI
{
    public ImageTextBehaviour ReqResourcesPrefab;

    [ComponentInject] private BuildingUiWrapperBehaviour BuildingUiWrapperBehaviour;


    void Start()
    {        
        var buildingTypeSettings = BuildingPrefabs.Get().Single(x => x.BuildingType == BuildingUiWrapperBehaviour.BuildingType);
        var itemsToBuildBuilding = buildingTypeSettings.BuildingPrefab.GetComponent<BuildingBehaviour>();
        if(itemsToBuildBuilding != null)
        {
            foreach(var reqItemToBuild in itemsToBuildBuilding.RequiredItems)
            {
                var reqItemCard = Instantiate(ReqResourcesPrefab, transform);
                reqItemCard.Text.text = reqItemToBuild.Amount.ToString();

                var itemSetting = ResourcePrefabs.Get().Single(x => x.ItemType == reqItemToBuild.ItemType);
                reqItemCard.Image.sprite = itemSetting.Icon;

                reqItemCard.ItemType = reqItemToBuild.ItemType;
            }
        }

    }
}