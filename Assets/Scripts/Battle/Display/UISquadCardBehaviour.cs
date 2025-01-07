using UnityEngine;
using UnityEngine.UI;

public class UISquadCardBehaviour : MonoBehaviour, IUiCardAddItemClick, IUiCardDecreaseItemClick
{
    public Image UnitImage;
    public Text CurrentCount;

    public Image DirectionArrow;

    private SquadBehaviour callingSquad => RtsUnitSelectionManager.Instance.CurrentSelected;
    
    void Update()
    {
        CurrentCount.text = "";
        if (callingSquad != null)
        {
            CurrentCount.text = callingSquad.UnitWidth.ToString();
            var newRotation = 360 - callingSquad.CurrentDirection.GetAngle();
            DirectionArrow.transform.rotation = Quaternion.Euler(DirectionArrow.transform.rotation.x, DirectionArrow.transform.rotation.y, newRotation); 
        }
    }

    public void AddAmount(int amount)
    {
        var newAmount = callingSquad.UnitWidth + 1;
        if(newAmount <= callingSquad.GetUnits().Count)
        {
            callingSquad.UnitWidth = newAmount;
        }

    }

    public void DecreaseAmount(int amount)
    {
        var newAmount = callingSquad.UnitWidth - 1;
        if (newAmount >= 1)
        {
            callingSquad.UnitWidth = newAmount;
        }
    }

    // button
    public void TurnDirectionRight()
    {
        callingSquad.CurrentDirection = callingSquad.CurrentDirection.TurnRight();
    }

    // button
    public void TurnDirectionLeft()
    {
        callingSquad.CurrentDirection = callingSquad.CurrentDirection.TurnLeft();
    }

    // button
    public void SetDefaultDirection()
    {
        RtsUnitSelectionManager.Instance.DefaultDirection = callingSquad.CurrentDirection;
    }
}