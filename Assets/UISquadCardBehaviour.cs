using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISquadCardBehaviour : MonoBehaviour, IUiCardAddItemClick, IUiCardDecreaseItemClick
{
    public Image UnitImage;
    public Text CurrentCount;

    private SquadBehaviour CallingSquad => RtsUnitSelectionManager.Instance.CurrentSelected;
    
    void Update()
    {
        CurrentCount.text = "";
        if (CallingSquad != null)
        {
            CurrentCount.text = CallingSquad.UnitWidth.ToString();
        }
    }

    public void AddAmount(int amount)
    {
        var newAmount = CallingSquad.UnitWidth + 1;
        if(newAmount <= CallingSquad.GetUnits().Count)
        {
            CallingSquad.UnitWidth = newAmount;
        }

    }

    public void DecreaseAmount(int amount)
    {
        var newAmount = CallingSquad.UnitWidth - 1;
        if (newAmount >= 1)
        {
            CallingSquad.UnitWidth = newAmount;
        }
    }
}
