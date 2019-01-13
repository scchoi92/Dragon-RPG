using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaAttackBehaviour : AbilityBehaviour
    {
        PlayerMovement player;

        private void Start()
        {
            player = GetComponent<PlayerMovement>();
        }

        public override void Use(GameObject target)
        {
            DealRadialDamage();
            PlayParticleEffect();
            PlaySFX();
            PlayAnimation();
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
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerMovement>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = player.GetDamageBeforeCritical() + (config as AreaAttackConfig).GetDamageToEachTarget();
                    damageable.AdjustHealth(damageToDeal);
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