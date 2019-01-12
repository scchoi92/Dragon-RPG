using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaAttackBehaviour : AbilityBehaviour
    {
        //private void Start()
        //{
        //    myAudioSource = GetComponent<AudioSource>();
        //}

        public override void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
            PlaySFX();
            PlayAnimation();
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                (config as AreaAttackConfig).GetEffectRadius(),
                Vector3.up,
                (config as AreaAttackConfig).GetEffectRadius()
                );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = useParams.baseDamage + (config as AreaAttackConfig).GetDamageToEachTarget();
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