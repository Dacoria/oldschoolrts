using UnityEngine;
using UnityEngine.UI;

public class UiQueueCardBehaviour : MonoBehaviour
{
    public Image Image;

    [HideInInspector] public UiQueueHandler DisplayQueueUIHandler;
    public GameObject CancelButtonGO;

    public void OnCancelClick()
    {
        DisplayQueueUIHandler.OnCancelQueueItemClick(this);
    }   
}