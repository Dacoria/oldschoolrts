using UnityEngine;

public class ScaleDownHarvestableMaterial : MonoBehaviour
{
    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    private void Awake()
    {
        this.ComponentInject();
    }

    private int updateCounter;

    void Update()
    {
        if(updateCounter == 0)
        {
            CheckScaleDown();
        }

        updateCounter++;
        if(updateCounter > 30)
        {
            updateCounter = 0;
        }
    }

    private void CheckScaleDown()
    {
        var scaleMultiplier = HarvestableMaterialScript.MaterialCount / (float)HarvestableMaterialScript.InitialMaterialCount;
        transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, scaleMultiplier);
        




    }
}
