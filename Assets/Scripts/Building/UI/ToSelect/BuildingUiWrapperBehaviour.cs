using UnityEngine;
using UnityEngine.UI;

public class BuildingUiWrapperBehaviour : MonoBehaviour
{
    public BuildingType BuildingType;
    public Image Image;
    public Text Text;
    public BuildingUiBehaviour BuildingUiBehaviour;
        
    public void BuildBuilding()
    {
        BuildingUiBehaviour.BuildBuilding(BuildingType);
    }
}