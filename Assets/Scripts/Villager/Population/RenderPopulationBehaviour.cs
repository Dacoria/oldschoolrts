using UnityEngine;
using UnityEngine.UI;

public class RenderPopulationBehaviour : MonoBehaviourCI
{
    public Text Text;

    [ComponentInject] private Image image;

    void Update()
    {
        Text.text = $"{PopulationManager.CurrentPopulation}/{PopulationManager.PopulationLimit}";

        if (PopulationManager.CurrentPopulation == PopulationManager.PopulationLimit)
        {
            this.image.color = Color.red;
        }
        else
        {
            this.image.color = Color.white;
        }
    }
}