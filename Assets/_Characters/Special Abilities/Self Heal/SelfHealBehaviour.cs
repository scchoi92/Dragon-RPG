using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        HealthSystem healthSystem;

        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
        }

        public override void Use(GameObject target)
        {
            ProcessHeal();
            PlayParticleEffect();
            PlaySFX();
            PlayAbilityAnimation();
        }

        private void ProcessHeal()
        {
            healthSystem.SetHealTimer((config as SelfHealConfig).GetDuration(), (config as SelfHealConfig).GetHealAmount());
        }
    }
}