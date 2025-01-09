using System;
using UnityEngine;
using UnityEngine.EventSystems;

public partial class UiManager : MonoBehaviour
{
    public Camera Camera;

    public GameObject SelectedBuildingPanel;
    public GameObject QueueUiGo;
    public MonoBehaviour BuildingOverviewUI;

    private DateTime leftMouseDownTimeNoUi;
    private DateTime leftMouseDownTimeAll;

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
            leftMouseDownTimeAll = DateTime.Now;
            if (!isClickingUi)
            {
                leftMouseDownTimeNoUi = DateTime.Now;
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            if (IsWorldClick())
            {
                DoLeftMouseClick();
            }
            else if(!IsAnyClick())
            {
                //drag
                DisableEntireCanvas();
            }
        }
    }

    private bool IsWorldClick() => (DateTime.Now - leftMouseDownTimeNoUi).TotalMilliseconds < 150;
    private bool IsAnyClick() => (DateTime.Now - leftMouseDownTimeAll).TotalMilliseconds < 150;

    private void DoLeftMouseClick()
    {        
        var ray = Camera.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray);
        var buildingHit = ActOnRaycastHit(hits);        
    }
}