using UnityEngine;

public class PopulationEligibleBehaviour : MonoBehaviour
{
    void Start()
    {
        PopulationManager.CurrentPopulation++;
    }

    public void OnDestroy()
    {
        PopulationManager.CurrentPopulation--;
    }
}
