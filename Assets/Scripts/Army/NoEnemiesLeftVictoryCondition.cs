using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoEnemiesLeftVictoryCondition : MonoBehaviour
{

    public GameObject Sprite;


    private int updateCounter;
    void Update()
    {
        if (updateCounter == 0 && this.GetComponentsInChildren<Transform>().Length <= 1)
        {
            this.Sprite.SetActive(true);
        }

        updateCounter++;
        if(updateCounter > 100)
        {
            updateCounter = 0;
        }
    }
}
