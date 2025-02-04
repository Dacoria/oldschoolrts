using UnityEngine;

namespace Assets.Army
{
    public static class HandleAttack
    {
        public static void Handle(Offence offence, GameObject defender)
        {
            var defence = defender.GetComponent<ArmyUnitBehaviour>().Defence;
            var damage = DamageLookup.CalculateAbsoluteDamage(offence, defence);
            defender.GetComponent<HealthBehaviour>().TakeDamage(damage);
        }     
    }
}