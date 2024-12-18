using UnityEngine;

public class ResourceCarryingBehaviour : MonoBehaviourCI
{
    public GameObject HarvestedResourceToCarryPrefab;
    private GameObject HarvestedResourceToCarry;

    public GameObject PlantResourceToCarryPrefab;
    private GameObject PlantResourceToCarry;

    [ComponentInject] private RetrieveResourceBehaviour RetrieveResourceScript;
    [ComponentInject(Required.OPTIONAL)] private PlantResourceBehaviour PlantResourceScript;

    void Start()
    {
        if (HarvestedResourceToCarryPrefab != null && RetrieveResourceScript != null)
        {
            HarvestedResourceToCarry = Instantiate(HarvestedResourceToCarryPrefab, this.transform, false);
            HarvestedResourceToCarry.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded
            HarvestedResourceToCarry.SetActive(false);

            if (PlantResourceToCarryPrefab != null && PlantResourceScript != null)
            {
                PlantResourceToCarry = Instantiate(PlantResourceToCarryPrefab, this.transform, false);
                PlantResourceToCarry.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded
                PlantResourceToCarry.SetActive(false);
            }
        }
    }        

    void Update()
    {
        if(RetrieveResourceScript != null)
        {
            var isCarryingResource = RetrieveResourceScript.IsCarryingResource();

            HarvestedResourceToCarry.SetActive(isCarryingResource);

            if(PlantResourceToCarry != null)
            {
                var isCarryingResource2 = PlantResourceScript.IsCarryingResource();
                PlantResourceToCarry.SetActive(!isCarryingResource && isCarryingResource2);
            }
        }
    }
}