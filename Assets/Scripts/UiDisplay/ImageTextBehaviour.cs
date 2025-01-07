using UnityEngine;
using UnityEngine.UI;

public class ImageTextBehaviour : MonoBehaviour
{
    public Image Image;
    public Text Text;

    public bool ActivateItemTooltip = true;

    private ItemType _itemType;
    [HideInInspector]
    public ItemType ItemType
    { 
        get { return _itemType; }
        set {
            _itemType = value;
            if(ActivateItemTooltip)
            {
                UpdateTooltip();
            }            
        }
    }

    private void UpdateTooltip()
    {
        var tooltip = gameObject.GetComponent<TooltipTriggerCanvas>();
        if (tooltip == null)
        {
            tooltip = gameObject.AddComponent<TooltipTriggerCanvas>();
        }

        tooltip.Content = ItemType.ToString().Capitalize();
    }
}