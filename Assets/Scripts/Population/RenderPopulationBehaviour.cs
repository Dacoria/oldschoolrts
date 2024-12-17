using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenderPopulationBehaviour : MonoBehaviour
{
    public Text Text;

    [ComponentInject] private Image image;

    void Awake()
    {
        this.ComponentInject();
    }

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
