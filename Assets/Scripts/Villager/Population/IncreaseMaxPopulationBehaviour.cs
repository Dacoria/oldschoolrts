using UnityEngine;

public class IncreaseMaxPopulationBehaviour : MonoBehaviour
{
    private int increasePopCount = 5;

    private void Start()
    {
        PopulationManager.PopulationLimit += increasePopCount;
    }

    private void OnDestroy()
    {
        PopulationManager.PopulationLimit -= increasePopCount;
    }
}