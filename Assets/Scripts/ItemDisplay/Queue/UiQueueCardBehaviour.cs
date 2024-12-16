using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiQueueCardBehaviour : MonoBehaviour
{
    public Image Image;

    [HideInInspector]
    public UiQueueHandler DisplayQueueUIHandler;

    public void OnCancelClick()
    {
        DisplayQueueUIHandler.OnCancelQueueItemClick(this);
    }   
}
