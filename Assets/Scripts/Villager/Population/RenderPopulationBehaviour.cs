using UnityEngine;
using UnityEngine.UI;

public class RenderPopulationBehaviour : MonoBehaviourCI
{
    public Text Text;

    [ComponentInject] private Image image;

    void Update()
    {
        Text.text = $"{GameManager.CurrentPopulation}/{GameManager.PopulationLimit}";

        if (GameManager.CurrentPopulation == GameManager.PopulationLimit)
        {
            this.image.color = Color.red;
        }
        else
        {
            this.image.color = Color.white;
        }
    }
}