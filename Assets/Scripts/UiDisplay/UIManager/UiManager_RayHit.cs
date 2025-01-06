using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private bool ActOnRaycastHit(RaycastHit[] hits)
    {
        (BuildingBehaviour buildingBehaviourHit, MonoBehaviour uiToActivate) = GetBuildingHitByRay(hits);

        if (buildingBehaviourHit != null && uiToActivate != null)
        {
            if (UIUserSettings.ShowExternalUIElementsActiveBuilding)            
                EnableExternalUIElementsActiveBuilding(buildingBehaviourHit.gameObject);
            
            EnableOutline(buildingBehaviourHit.gameObject);
            ActiveRangedDisplay(buildingBehaviourHit.gameObject);
            ActivateUI(uiToActivate);
            return true;
        }
        else
        {            
            DisableEntireCanvas();
            foreach (var hit in hits)
            {
                AE.LeftClickOnGo?.Invoke(hit.transform.gameObject); // nu gebruikt voor tooltips
            }

            return false;
        }
    }

    public void ActivateUI(MonoBehaviour ui)
    {
        DisableEntireCanvas();
        SelectedBuildingPanel.SetActive(true);

        if (ui != null)
        {
            ActivateQueueIfNeeded(ui);
            ui.gameObject.SetActive(true);
            ui.gameObject.transform?.parent?.gameObject.SetActive(true);
        }
    }
}