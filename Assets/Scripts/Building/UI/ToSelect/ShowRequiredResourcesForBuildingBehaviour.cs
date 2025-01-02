using System.Linq;

public class ShowRequiredResourcesForBuildingBehaviour : MonoBehaviourCI
{
    public ImageTextBehaviour ReqResourcesPrefab;

    [ComponentInject] private BuildingUiWrapperBehaviour BuildingUiWrapperBehaviour;

    void Start()
    {
        var buildCosts = BuildingUiWrapperBehaviour.BuildingType.GetBuildCosts();        
        foreach(var reqItemToBuild in buildCosts)
        {
            var reqItemCard = Instantiate(ReqResourcesPrefab, transform);
            reqItemCard.Text.text = reqItemToBuild.Amount.ToString();

            var itemSetting = ResourcePrefabs.Get().Single(x => x.ItemType == reqItemToBuild.ItemType);
            reqItemCard.Image.sprite = itemSetting.Icon;

            reqItemCard.ItemType = reqItemToBuild.ItemType;
        }        
    }
}