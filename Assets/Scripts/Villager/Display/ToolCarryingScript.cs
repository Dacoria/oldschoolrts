using UnityEngine;

public class ToolCarryingScript : MonoBehaviour
{
    public GameObject ToolToShowHide;
    [ComponentInject] private IToolShowVillager toolShowVillager;

    void Start()
    {
        this.ComponentInject(); // in awake worden pas comp. toegevoegd
        if(toolShowVillager == null)
        {
            throw new System.Exception("Script verwacht een interface in dit gameobject met een IToolShowVillager interface");
        }

        if (ToolToShowHide == null)
        {
            throw new System.Exception("Geen GO gevonden om te showen of te hiden");
        }
    }

    void Update()
    {
        ToolToShowHide.SetActive(toolShowVillager.ShouldShowTool());
    }
}