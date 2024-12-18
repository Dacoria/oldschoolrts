using Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UiTavernRefillHandler : MonoBehaviour
{
    // setten van queue -> dit zorgt dat de queue ververst voor het gebouw
    [HideInInspector]
    public TavernRefillingBehaviour CallingTavernRefillingBehaviour;

    // display queue (weergave van items)
    private List<UiTavernRefillItem> UiRefillItems;
    private UiTavernBehaviour UiTavernBehaviour;

    public class UiTavernRefillItem
    {
        public UiTavernRefillCardBehaviour UiTavernRefillCardBehaviour;

        private TavernRefillItem _tavernRefillItem;
        public TavernRefillItem TavernRefillItem
        {
            get => _tavernRefillItem;
            set
            {
                var previousValue = _tavernRefillItem;
                _tavernRefillItem = value;
                UiTavernRefillCardBehaviour.SelectedTavernQueueItem = value;
            }
        }
    }

    public void Awake()
    {
        UiTavernBehaviour = transform.parent.parent.GetComponentInChildren<UiTavernBehaviour>();

        UiRefillItems = new List<UiTavernRefillItem>();
        var foodRefillCards = GetComponentsInChildren<UiTavernRefillCardBehaviour>();
        foreach (var foodRefillCard in foodRefillCards)
        {
            var item = new UiTavernRefillItem
            {
                UiTavernRefillCardBehaviour = foodRefillCard,
                TavernRefillItem = null
            };
            UiRefillItems.Add(item);
        }
    }
    

    private int updateCounter = 0;
    private TavernBehaviour LastTavernBehaviour;

    public void Update()
    {
        if(UiTavernBehaviour.CallingTavern != LastTavernBehaviour)
        {
            CallingTavernRefillingBehaviour = UiTavernBehaviour.CallingTavern.GetComponent<TavernRefillingBehaviour>();
        }

        if (updateCounter == 0)
        {
            UpdateRefilling();
        }
        updateCounter++;
        if (updateCounter > 10)
        {
            updateCounter = 0;
        }
    }

    private void UpdateRefilling()
    {
        if(CallingTavernRefillingBehaviour == null)
        {
            return;
        }
        

        for(var i = 0; i < FoodConsumptionSettings.Tavern_Max_Refill_Items; i++)
        {
            var displayItem = UiRefillItems[i]; // gelijk aan de settings

            if (i < CallingTavernRefillingBehaviour.TavernRefillItems.Count)
            {
                var refillItem = CallingTavernRefillingBehaviour.TavernRefillItems[i];                

                if (refillItem != displayItem.TavernRefillItem)
                {
                    displayItem.TavernRefillItem = refillItem; // dit update automatisch de displays
                }
            }
            else
            {
                displayItem.TavernRefillItem = null; // dit update automatisch de displays
            }

        }
    }
}
