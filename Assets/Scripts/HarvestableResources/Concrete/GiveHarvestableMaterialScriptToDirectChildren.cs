using UnityEngine;

public class GiveHarvestableMaterialScriptToDirectChildren : MonoBehaviour
{
    public MaterialResourceType MaterialType;
    public int MaterialCount;

    public bool GiveTooltipScript;

    void Start()
    {
        if(MaterialType == MaterialResourceType.NONE) { throw new System.Exception("MaterialType is verplicht"); }

        foreach (Transform child in transform)
        {

            child.gameObject.AddComponent<HarvestableMaterialScript>();
            var addedScript = child.GetComponent<HarvestableMaterialScript>();
            addedScript.MaterialCount = MaterialCount;
            addedScript.MaterialType = MaterialType;

            if (GiveTooltipScript)
            {
                child.gameObject.AddComponent<TooltipHarvestableMaterialScript>();
            }
        }
    }
}