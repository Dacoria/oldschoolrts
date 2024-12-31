using UnityEngine;

public class RotateGearsScript : MonoBehaviourCI
{
    public GameObject GearBig;
    public GameObject GearSmall;

    public Material GearsRunningMaterial;
    public Material GearsNotRunningMaterial;

    public bool RotateGears;
    private Renderer[] RenderersChildren;

    [ComponentInject] private HandleProduceResourceOrderOverTimeBehaviour ProduceResourceBehaviour;

    private void Start()
    {
        RenderersChildren = GetComponentsInChildren<Renderer>();
        UpdateRenderers();
        if(ProduceResourceBehaviour == null)
        {
            throw new System.Exception("Gears animatie vereist ProduceResourceBehaviour/QueueForBuildingBehaviour in parent!");
        }
    }

    public void RunGears()
    {
        RotateGears = true;
        UpdateRenderers();
    }

    public void StopGears()
    {
        RotateGears = false;
        UpdateRenderers();
    }

    private void UpdateRenderers()
    {
        foreach (var renderer in RenderersChildren)
        {
            renderer.material = RotateGears ? GearsRunningMaterial : GearsNotRunningMaterial;
        }
    }

    private bool previousRotateGears;

    void Update()
    {
        if(ProduceResourceBehaviour != null)
            RotateGears = ProduceResourceBehaviour.IsProducingResourcesRightNow;        

        if (RotateGears)
        {        
            GearBig.transform.Rotate(new Vector3(0, 0.1f, 0));
            GearSmall.transform.Rotate(new Vector3(0, -0.1f, 0));
        }        

        if(RotateGears != previousRotateGears)
        {
            UpdateRenderers();
        }

        previousRotateGears = RotateGears;
    }
}