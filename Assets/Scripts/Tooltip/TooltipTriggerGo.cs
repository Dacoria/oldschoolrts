using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipTriggerGo : MonoBehaviour, ITooltipUIText
{
    public string Content;
    public string Header;

    public void Awake()
    {
        gameObject.AddComponent<TooltipUIHandler>();
    }
    public string GetContentText() => Content;

    public string GetHeaderText() => Header;

    //public void OnMouseEnter()
    //{  
    //    TooltipSystem.instance.Show(Content, Header);
    //}
    //
    //public void OnMouseExit()
    //{        
    //    TooltipSystem.instance.Hide();
    //}
}
