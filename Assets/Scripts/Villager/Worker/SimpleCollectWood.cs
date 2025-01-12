using UnityEngine;

public class SimpleCollectWood : MonoBehaviour
{
    public int NumberOfWood; // via ui instellen

    public void WoodDroppedOff()
    {
        // nu hardcoded aangeroepen door villager
        NumberOfWood = NumberOfWood + 1;
    }
}