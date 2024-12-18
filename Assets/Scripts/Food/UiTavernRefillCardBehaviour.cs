using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UiTavernRefillCardBehaviour : MonoBehaviour
{
    public Image FoodOnQueueImage;
    public Image VillagerOnQueueImage;
    public Text ToHealthPercentageText;

    private TavernRefillItem _selectedTavernQueueItem;
    public TavernRefillItem SelectedTavernQueueItem
    {
        get => _selectedTavernQueueItem;
        set
        {            
            var previousValue = _selectedTavernQueueItem;
            _selectedTavernQueueItem = value;

            if (_selectedTavernQueueItem != null)
            {
                FoodOnQueueImage.transform.parent.gameObject.SetActive(true);
                VillagerOnQueueImage.transform.parent.gameObject.SetActive(true);

                FoodOnQueueImage.sprite = GetFoodOnQueueIcon(_selectedTavernQueueItem.FoodType);
                VillagerOnQueueImage.sprite = GetVillagerOnQueueIcon(_selectedTavernQueueItem.VillagerType);
                ToHealthPercentageText.text = GetVillagerHealthText(_selectedTavernQueueItem.FoodConsumptionBehaviour, _selectedTavernQueueItem.FoodType);
            }
            else
            {
                FoodOnQueueImage.transform.parent.gameObject.SetActive(false);
                VillagerOnQueueImage.transform.parent.gameObject.SetActive(false);
            }
                        
        }
    }

    private Sprite GetFoodOnQueueIcon(ItemType foodType)
    {
        var itemSettings = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == foodType);
        return itemSettings.Icon;
    }

    private Sprite GetVillagerOnQueueIcon(VillagerUnitType villagerType)
    {
        var schoolPrefab = GameManager.Instance.BuildingPrefabItems.Single(x => x.BuildingType == BuildingType.SCHOOL).BuildingPrefab;
        var schoolbehaviour = schoolPrefab.GetComponentInChildren<SchoolBehaviour>(true);
        var villager = schoolbehaviour.VillagerUnitSettings.Single(x => x.Type == villagerType);
        return villager.Icon;
    }

    private string GetVillagerHealthText(FoodConsumptionBehaviour foodConsumptionBehaviour, ItemType foodType)
    {
        var foodSettings = GameManager.Instance.ItemFoodRefillValues.Single(x => x.ItemType == foodType);
        var foodHealthBoost = foodSettings.RefillValue;

        var percAfterFoodConsumption = Math.Min(100, (int)Math.Round((foodConsumptionBehaviour.FoodSatisfactionPercentage + foodHealthBoost) * 100));
        return percAfterFoodConsumption + "%";
    }
}