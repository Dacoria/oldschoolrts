using UnityEngine;

public class PopulationEligibleBehaviour : MonoBehaviour
{
    void Start()
    {
        GameManager.CurrentPopulation++;
    }

    public void OnDestroy()
    {
        GameManager.CurrentPopulation--;
    }
}
