using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UiTavernBehaviour : MonoBehaviourCI, ICardCarousselDisplay
{
    public TavernUiWrapperBehaviour TavernUiWrapperBehaviourPrefab;

    [HideInInspector]
    public List<TavernUiWrapperBehaviour> TavernUiWrapperBehaviours;

    [HideInInspector]
    public TavernBehaviour CallingTavern;

    private bool CardsLoaded;

    private void Start()
    {
        TavernUiWrapperBehaviours = new List<TavernUiWrapperBehaviour>();
        foreach (var itemFoodSetting in GameManager.Instance.ItemFoodRefillValues)
        {
            var tavernUiWrapper = Instantiate(TavernUiWrapperBehaviourPrefab, transform);
            var resourceItemSettings = GameManager.Instance.ResourcePrefabItems.Single(x => x.ItemType == itemFoodSetting.ItemType);
            tavernUiWrapper.Image.sprite = resourceItemSettings.Icon;

            tavernUiWrapper.ItemType = itemFoodSetting.ItemType;
            tavernUiWrapper.TavernUiBehaviour = this;
            tavernUiWrapper.TitleText.text = Char.ToLowerInvariant(tavernUiWrapper.ItemType.ToString()[0]).ToString().ToUpper() + tavernUiWrapper.ItemType.ToString().ToLower() .Substring(1);
            TavernUiWrapperBehaviours.Add(tavernUiWrapper);
        }

        CardsLoaded = true;
    }


    public int GetCount() => TavernUiWrapperBehaviours.Count;

    public bool CardsAreLoaded() => CardsLoaded;

    public void SetActiveStatusCardGo(int indexOfCard, bool activeYN)
    {
        TavernUiWrapperBehaviours[indexOfCard].gameObject.SetActive(activeYN);
    }

    public int GetIndexFirstEnabledCard()
    {
        for (int i = 0; i < TavernUiWrapperBehaviours.Count; i++)
        {
            var card = TavernUiWrapperBehaviours[i];
            if (card.gameObject.activeSelf)
            {
                return i;
            }
        }

        return -1;
    }
}