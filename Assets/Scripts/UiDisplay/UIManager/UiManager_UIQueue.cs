using UnityEngine;

public partial class UiManager : MonoBehaviour
{
    private void ActivateQueueIfNeeded(MonoBehaviour uiToActivate)
    {
        if (uiToActivate is CardSelectProdUiHandler)
        {
            var cardHandler = (CardSelectProdUiHandler)uiToActivate;
            var queue = cardHandler?.CallingBuilding?.GetGameObject()?.GetComponent<QueueForBuildingBehaviour>();
            if (queue != null)
            {
                QueueUiGo.GetComponentInChildren<UiQueueHandler>(true).CallingQueueForBuildingBehaviour = queue;
                QueueUiGo.SetActive(true);
                return;
            }
        }

        QueueUiGo.SetActive(false);
    }
}