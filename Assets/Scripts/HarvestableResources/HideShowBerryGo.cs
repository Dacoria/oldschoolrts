using UnityEngine;

public class HideShowBerryGo : MonoBehaviour
{
    public GameObject HasBerries;
    public GameObject NoBerries;

    [ComponentInject]
    private HarvestableMaterialScript HarvestableMaterialScript;

    private int checkCounter;

    private void Awake()
    {
        this.ComponentInject();
    }

    void Update()
    {
        if (checkCounter == 0)
        {
            CheckToHideShowBerries();
        }

        checkCounter++;
        if (checkCounter >= 100)
        {
            checkCounter = 0;
        }
    }

    private void CheckToHideShowBerries()
    {
        if (HarvestableMaterialScript.MaterialCount == 0 && !NoBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
        if (HarvestableMaterialScript.MaterialCount > 0 && !HasBerries.activeSelf)
        {
            UpdateActiveBerriesGos();
        }
    }

    private void UpdateActiveBerriesGos()
    {
        NoBerries.SetActive(HarvestableMaterialScript.MaterialCount == 0);
        HasBerries.SetActive(HarvestableMaterialScript.MaterialCount != 0);
    }
}
