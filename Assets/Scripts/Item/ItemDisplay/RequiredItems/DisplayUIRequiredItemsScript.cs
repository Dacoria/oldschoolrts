using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DisplayUIRequiredItemsScript : MonoBehaviour
{
    private List<ImageTextBehaviour> ImagesRequiredItems;

    private void Start()
    {
        // voor kaartjes onder armor/weapons (req) wel nodig, niet voor wanneer je in het groot 1 kaart wilt tonen die je hebt aangeklikt
        var cardBehaviourInParent = transform.parent.GetComponent<UiCardBehaviour>();
        if (cardBehaviourInParent != null)
        {
            UpdateSelectedCard(cardBehaviourInParent);
        }
    }

    public void UpdateSelectedCard(UiCardBehaviour selectedDisplayUiCard)
    {
        // selectedDisplayCard altijd de req items tonen
        var isSelectedDisplayCard = transform.GetComponentInParent<SelectedDisplayCardScript>() != null;
        if (isSelectedDisplayCard || (selectedDisplayUiCard?.CardUiHandler != null && selectedDisplayUiCard.CardUiHandler.ShowRequiredItemsUnderCard))
        {
            ImagesRequiredItems = GetComponentsInChildren<ImageTextBehaviour>(true).ToList();
            var buildingType = selectedDisplayUiCard.CardUiHandler.CallingBuilding.GetBuildingType();
            var produceSetting = buildingType.GetProductionSettings().First(x => x.GetType().ToString() == selectedDisplayUiCard.Type.ToString());
            UpdateImages(produceSetting);
        }
        else
        {
            transform.gameObject.SetActive(false);
        }
    }       

    private void UpdateImages(ProductionSetting productionSetting)
    {
        foreach(var imageScript in ImagesRequiredItems)
        {
            imageScript.gameObject.SetActive(false);
        }

        for(int i = 0; i < productionSetting.ItemsConsumedToProduce.Count; i++)
        {
            var prodSettingForItemTypeRequired = productionSetting.ItemsConsumedToProduce[i];
            var prefabSettingForItemTypeRequired = ResourcePrefabs.Get().Single(x => x.ItemType == prodSettingForItemTypeRequired.ItemType);

            SetImageAndActiveGoForItemConsumed(prodSettingForItemTypeRequired, prefabSettingForItemTypeRequired);            
        }
    }

    private void SetImageAndActiveGoForItemConsumed(ItemAmountBuffer prodSettingForItemTypeRequired, ResourcePrefabItem prefabSettingForItemTypeRequired)
    {
        // loop voor als je voor 1 item 2 items nodig hebt -> dan toon je 2 plaatjes
        for (int j = 0; j < prodSettingForItemTypeRequired.Amount; j++)
        {
            var imageFound = false;
            foreach (var itemScript in ImagesRequiredItems)
            {                
                if(!itemScript.gameObject.activeSelf)
                {
                    itemScript.gameObject.SetActive(true);
                    itemScript.Image.sprite = prefabSettingForItemTypeRequired.Icon;
                    itemScript.ItemType = prodSettingForItemTypeRequired.ItemType;
                    imageFound = true;
                    break;
                }
            }
            if (!imageFound)
            {
                throw new Exception($"Item benodigd display kan max {ImagesRequiredItems.Count} items tonen, item {prodSettingForItemTypeRequired.ItemType} vereist er meer, dat past niet");
            }
        }
    }
}