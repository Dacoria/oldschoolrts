using UnityEngine;

public class ResourceCarryingBehaviour : MonoBehaviour
{
    public GameObject HarvestedResourceToCarryPrefab;
    private GameObject harvestedResourceToCarry;

    public GameObject PlantResourceToCarryPrefab;
    private GameObject plantResourceToCarry;

    [ComponentInject] private RetrieveResourceBehaviour retrieveResourceScript;
    [ComponentInject(Required.OPTIONAL)] private PlantResourceBehaviour plantResourceScript;

    void Start()
    {
        this.ComponentInject(); // in awake van villager worden componenten pas toegevoegd
        if (HarvestedResourceToCarryPrefab != null && retrieveResourceScript != null)
        {
            harvestedResourceToCarry = Instantiate(HarvestedResourceToCarryPrefab, this.transform, false);
            harvestedResourceToCarry.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded
            harvestedResourceToCarry.SetActive(false);

            if (PlantResourceToCarryPrefab != null && plantResourceScript != null)
            {
                plantResourceToCarry = Instantiate(PlantResourceToCarryPrefab, this.transform, false);
                plantResourceToCarry.transform.localPosition = new Vector3(0, 1.2f, 0.5f); // net voor de borst -> voor nu hardcoded
                plantResourceToCarry.SetActive(false);
            }
        }
    }        

    void Update()
    {
        if(retrieveResourceScript != null)
        {
            var isCarryingResource = retrieveResourceScript.IsCarryingResource();

            harvestedResourceToCarry.SetActive(isCarryingResource);

            if(plantResourceToCarry != null)
            {
                var isCarryingResource2 = plantResourceScript.IsCarryingResource();
                plantResourceToCarry.SetActive(!isCarryingResource && isCarryingResource2);
            }
        }
    }
}