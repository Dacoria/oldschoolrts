using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    // via button click
    public void ActivateUi(MonoBehaviour mono)
    {
        ActivateUI(mono);
/*

        if (mono.gameObject.name == "SkillTree")
        {
            // skilltree zit in een andere canvas
            ActivateUI(true, mono, SelectedBuildingPanelSkillTree);
        }
        else
        {
            ActivateUI(true, mono);
        }*/
    }
}