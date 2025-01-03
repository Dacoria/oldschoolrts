using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    public OutlineBehaviour ActiveOutlineBehaviour;    

    private void EnableOutline(GameObject go)
    {
        ActiveOutlineBehaviour = go.GetComponent<OutlineBehaviour>();
        if (ActiveOutlineBehaviour != null)
        {
            ActiveOutlineBehaviour.enabled = true;
        }
        else
        {
            ActiveOutlineBehaviour = go.AddComponent<OutlineBehaviour>();
        }
    }

    private void DisableActiveOutline()
    {
        if (ActiveOutlineBehaviour != null)
        {
            ActiveOutlineBehaviour.enabled = false;
        }
    }
}