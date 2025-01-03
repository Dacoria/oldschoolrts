using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    public Camera Camera;

    public GameObject SelectedBuildingPanel;
    public GameObject SelectedBuildingPanelSkillTree;
    public GameObject QueueUiGo;
    public MonoBehaviour BuildingOverviewUI;

    public RangeDisplayBehaviour ActiveRangeDisplayBehaviour;
    public DisplayBuildingInputOutputHandler ActiveDisplayBuildingInputOutputHandler;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            DisableActiveBuilding();
            SelectedBuildingPanel.SetActive(false);
            SelectedBuildingPanelSkillTree.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0))
        {
            DisableActiveBuilding();
            var isClickingUi = EventSystem.current.IsPointerOverGameObject();

            if (!isClickingUi)
            {
                Ray ray = Camera.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray);
                ActOnRaycastHit(hits);
            }
        }
    }
}