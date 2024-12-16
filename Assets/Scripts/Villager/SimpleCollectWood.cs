using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCollectWood : MonoBehaviour
{
    public int NumberOfWood;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WoodDroppedOff()
    {
        // nu hardcoded aangeroepen door villager
        NumberOfWood = NumberOfWood + 1;
    }
}
