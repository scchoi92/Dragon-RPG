using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        PlayerMovement player;

        private void Start()
        {
            player = GetComponent<PlayerMovement>();
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
            float damageToDeal = player.GetDamageBeforeCritical() + (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().AdjustHealth(damageToDeal);
        }
    }
}