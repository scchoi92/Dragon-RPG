using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {
        //AudioSource myAudioSource;

        //private void Start()
        //{
        //    myAudioSource = GetComponent<AudioSource>();
        //}

        public override void Use(AbilityUseParams useParams)
        {
            DealDamage(useParams);
            PlayParticleEffect();
            PlaySFX();
            PlayAnimation();
        }

        private void DealDamage(AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage();
            useParams.target.AdjustHealth(damageToDeal);
        }
    }
}