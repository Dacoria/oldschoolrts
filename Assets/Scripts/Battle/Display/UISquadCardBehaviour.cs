using UnityEngine;
using UnityEngine.UI;

public class UISquadCardBehaviour : MonoBehaviour, IUiCardAddItemClick, IUiCardDecreaseItemClick
{
    public Image UnitImage;
    public Text CurrentCount;

    public Image DirectionArrow;

    private SquadBehaviour CallingSquad => RtsUnitSelectionManager.Instance.CurrentSelected;
    
    void Update()
    {
        CurrentCount.text = "";
        if (CallingSquad != null)
        {
            CurrentCount.text = CallingSquad.UnitWidth.ToString();
            var newRotation = 360 - CallingSquad.CurrentDirection.GetAngle();
            DirectionArrow.transform.rotation = Quaternion.Euler(DirectionArrow.transform.rotation.x, DirectionArrow.transform.rotation.y, newRotation); 
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

    // button
    public void TurnDirectionRight()
    {
        CallingSquad.CurrentDirection = CallingSquad.CurrentDirection.TurnRight();
    }

    // button
    public void TurnDirectionLeft()
    {
        CallingSquad.CurrentDirection = CallingSquad.CurrentDirection.TurnLeft();
    }

    // button
    public void SetDefaultDirection()
    {
        RtsUnitSelectionManager.Instance.DefaultDirection = CallingSquad.CurrentDirection;
    }
}