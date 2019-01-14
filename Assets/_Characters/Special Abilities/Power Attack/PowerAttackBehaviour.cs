using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        WeaponSystem weaponSystem;

        private void Start()
        {
            weaponSystem = GetComponent<WeaponSystem>();
        }

        public override void Use(GameObject target)
        {
            DealDamage(target);
            PlayParticleEffect();
            PlaySFX();
            PlayAnimation();
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = weaponSystem.GetDamageBeforeCritical() + (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().AdjustHealth(damageToDeal);
        }
    }
}