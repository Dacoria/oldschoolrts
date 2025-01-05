using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    public Camera Camera;

    public GameObject SelectedBuildingPanel;
    public GameObject QueueUiGo;
    public MonoBehaviour BuildingOverviewUI;

    [HideInInspector] public RangeDisplayBehaviour ActiveRangeDisplayBehaviour;
    [HideInInspector] public DisplayBuildingInputOutputHandler ActiveDisplayBuildingInputOutputHandler;

    void Update()
    {
        var isClickingUi = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(1))
        {
            DisableActiveBuilding();
            SelectedBuildingPanel.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            DisableActiveBuilding();

            if (!isClickingUi)
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray);
                var buildingHit = ActOnRaycastHit(hits);
            }
        }        
    }
}