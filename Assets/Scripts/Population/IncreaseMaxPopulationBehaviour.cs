using UnityEngine;

public class IncreaseMaxPopulationBehaviour : MonoBehaviour
{
    public int IncreasePopCount = 5;

    private void Start()
    {
        GameManager.PopulationLimit += IncreasePopCount;
    }

    private void OnDestroy()
    {
        GameManager.PopulationLimit -= IncreasePopCount;
    }
}