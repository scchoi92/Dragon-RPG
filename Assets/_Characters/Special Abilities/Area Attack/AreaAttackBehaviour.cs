using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaAttackBehaviour : AbilityBehaviour
    {
        WeaponSystem weaponSystem;

        private void Start()
        {
            weaponSystem = GetComponent<WeaponSystem>();
        }

        public override void Use(GameObject target)
        {
            DealRadialDamage();
            PlayParticleEffect();
            PlaySFX();
            PlayAbilityAnimation();
        }

        private void DealRadialDamage()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                (config as AreaAttackConfig).GetEffectRadius(),
                Vector3.up,
                (config as AreaAttackConfig).GetEffectRadius()
                );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = weaponSystem.GetDamageBeforeCritical() + (config as AreaAttackConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }

        //private void PlaySFX()
        //{
        //    myAudioSource.clip = config.GetSFX(); // TODO find way of moving audio to parent class
        //    myAudioSource.volume = 0.15f;
        //    myAudioSource.Play();
        //}

    }
}