using UnityEngine;
using UnityEngine.UI;
using System;

public class UiCardBehaviour : MonoBehaviour, IUiCardAddNewItemsClick, IUiCardLeftClick
{
    public Image Image;
    public Text CountText;
    public Text TitleText;
    public Enum Type;

    [HideInInspector]
    public CardUiHandler CardUiHandler;

    public void AddAmount(int amount)
    {
        CardUiHandler.AddAmount(this.Type, amount);
    }

    public void DecreaseAmount(int amount)
    {
        CardUiHandler.DecreaseAmount(this.Type, amount);       
    }

    public void ClickOnCardLeftClick()
    {
        CardUiHandler.ClickOnCardLeftClick(this);
    }  

    private void Update()
    {
        if (CardUiHandler != null && CountText?.text != null)
        {
            CountText.text = CardUiHandler.GetCount(this.Type).ToString();
        }

    }
}