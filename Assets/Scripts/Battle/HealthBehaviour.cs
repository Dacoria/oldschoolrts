using System;
using UnityEngine;

public class HealthBehaviour : BaseAEMonoCI
{
    [HideInInspector] private float initialHeath = 100; // wordt geset bij barracksunits via de unitsettings in armybehaviour

    public float CurrentHealth = 100;
    public float DamageForNoFood = 1;

    public GameObject InstantiatedGoOnDeath;

    private bool isDying;

    [ComponentInject(Required.OPTIONAL)] private FoodConsumptionBehaviour foodConsumptionBehaviour;

    [ComponentInject(Required.OPTIONAL)] private IVillagerUnit villagerUnit;
    [ComponentInject(Required.OPTIONAL)] private ArmyUnitBehaviour armyUnitBehaviour;

    void Start()
    {
        SetInitialHealth();
        CurrentHealth = initialHeath;
    }

    private void SetInitialHealth()
    {
        if (villagerUnit != null)
        {
            initialHeath = 100;
        }
        else if (armyUnitBehaviour != null)
        {
            initialHeath = armyUnitBehaviour.BarracksUnitType.GetUnitStats().Health;
        }
        else
        {
            throw new Exception("Geen unit voor init health");
        }
    }

    protected override void OnNoFoodToConsume(FoodConsumption foodConsumption)
    {
        if(foodConsumption == foodConsumptionBehaviour?.FoodConsumption)
        {
            TakeDamage(DamageForNoFood);
        }
    }

    public void RecoverHealth(float healthAmount)
    {
        CurrentHealth += healthAmount;
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if(CurrentHealth == 0 && !isDying)
        {
            Die();
        }
    }

    private void Die()
    {
        isDying = true;
        if (InstantiatedGoOnDeath != null)
        {
            var instantiatedDeathGo = Instantiate(InstantiatedGoOnDeath, this.transform.position, this.transform.rotation);
        }
        Destroy(this.gameObject);
    }   
}