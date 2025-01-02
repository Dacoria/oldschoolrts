using UnityEngine;

public class IncreaseMaxPopulationBehaviour : MonoBehaviour
{
    public int IncreasePopCount = 5;

    private void Start()
    {
        PopulationManager.PopulationLimit += IncreasePopCount;
    }

    private void OnDestroy()
    {
        PopulationManager.PopulationLimit -= IncreasePopCount;
    }
}