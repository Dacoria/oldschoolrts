using UnityEngine;
using UnityEngine.UI;

public class SelectedDisplayCardScript : MonoBehaviour
{
    public Image Image;
    public GameObject BigCard;

    public DisplayUIRequiredItemsScript DisplayUIRequiredItemsScript;

    private UiCardBehaviour _selectedDisplayUiCard;
    public UiCardBehaviour SelectedDisplayUiCard
    {
        get => _selectedDisplayUiCard;
        set
        {            
            var previousValue = _selectedDisplayUiCard;
            _selectedDisplayUiCard = value;
            transform.gameObject.SetActive(true);

            BigCard.SetActive(true);

            Image.sprite = _selectedDisplayUiCard.Image.sprite;
            DisplayUIRequiredItemsScript.UpdateSelectedCard(_selectedDisplayUiCard);            
        }
    }

    public void OnDisable()
    {
        BigCard.SetActive(false);
    }
}