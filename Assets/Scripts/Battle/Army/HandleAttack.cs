using UnityEngine;

namespace Assets.Army
{
    public static class HandleAttack
    {
        private static float baseToHit = 35;

        public static void Handle(Offence offence, GameObject defender)
        {
            var defence = defender.GetComponent<ArmyUnitBehaviour>().Defence;

            if (AttackHits(offence, defence))
            {
                var damage = DamageLookup.CalculateAbsoluteDamage(offence, defence);
                defender.GetComponent<HealthBehaviour>().TakeDamage(damage);
            }
        }

        private static bool AttackHits(Offence offence, Defence defence)
        {
            var roll = Random.Range(0, 100);
            return (baseToHit + offence.AttackHitRate - defence.Dodge) > roll;
        }
    }
}