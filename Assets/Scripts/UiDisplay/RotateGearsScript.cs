using UnityEngine;

public class RotateGearsScript : MonoBehaviourCI
{
    public GameObject GearBig;
    public GameObject GearSmall;

    public Material GearsRunningMaterial;
    public Material GearsNotRunningMaterial;

    public bool RotateGears;
    private Renderer[] renderersChildren;

    [ComponentInject] private ProduceCRBehaviour produceBehaviour;

    private void Start()
    {
        renderersChildren = GetComponentsInChildren<Renderer>();
        UpdateRenderers();
        if(produceBehaviour == null)
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
        foreach (var renderer in renderersChildren)
        {
            renderer.material = RotateGears ? GearsRunningMaterial : GearsNotRunningMaterial;
        }
    }

    private bool previousRotateGears;

    void Update()
    {
        if(produceBehaviour != null)
            RotateGears = produceBehaviour.IsProducingResourcesRightNow;        

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