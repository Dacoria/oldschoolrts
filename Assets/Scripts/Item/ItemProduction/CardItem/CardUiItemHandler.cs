using Assets;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CardUiItemHandler : MonoBehaviour
{
    public ItemType ItemType;
    public Text CountText;

    public ICardBuilding CallingBuilding;

    public void AddItemType(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CallingBuilding.AddItem(ItemType);
        }
    }

    public void DecreaseItemType(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CallingBuilding.DecreaseItem(ItemType);
        }
    }

    private void Update()
    {
        if(CallingBuilding != null)
        {
            CountText.text = CallingBuilding.GetCount(ItemType).ToString();
        }
        
    }
}
