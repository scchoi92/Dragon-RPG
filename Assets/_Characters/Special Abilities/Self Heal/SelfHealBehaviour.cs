using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player;

        private void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            ProcessHeal(useParams);
            PlayParticleEffect();
            PlaySFX();
        }

        private void ProcessHeal(AbilityUseParams useParams)
        {
            player.SetHealTimer((config as SelfHealConfig).GetDuration(), (config as SelfHealConfig).GetHealAmount());
        }
    }
}