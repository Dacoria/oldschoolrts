using UnityEngine;

public class HealthBehaviour : BaseAEMonoCI
{
    [HideInInspector] public float InitialHeath = 100;

    public float CurrentHealth = 100;
    public float DamageForNoFood = 1;

    public GameObject InstantiatedGoOnDeath;

    private FoodConsumption FoodConsumption;
    private bool isDying;

    [ComponentInject(Required.OPTIONAL)] private FoodConsumptionBehaviour FoodConsumptionBehaviour;

    void Start()
    {
        InitialHeath = CurrentHealth;
        if (FoodConsumptionBehaviour != null)
        {
            FoodConsumption = FoodConsumptionBehaviour.FoodConsumption;
        }
    }

    protected override void OnNoFoodToConsume(FoodConsumption foodConsumption)
    {
        if(FoodConsumption == foodConsumption)
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
        if(CurrentHealth <= 0 && !isDying)
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