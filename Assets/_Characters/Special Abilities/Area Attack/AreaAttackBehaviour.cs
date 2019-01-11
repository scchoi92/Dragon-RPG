using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class AreaAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaAttackConfig config;

        public void SetConfig(AreaAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        private void Start()
        {
        }

        public void Use(AbilityUseParams useParams)
        {
            DealRadialDamage(useParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                config.GetEffectRadius(),
                Vector3.up,
                config.GetEffectRadius()
                );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                    damageable.AdjustHealth(damageToDeal);
                }
            }
        }
    }
}