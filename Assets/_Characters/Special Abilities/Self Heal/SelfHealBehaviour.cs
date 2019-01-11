using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;
using System;

namespace RPG.Characters
{
    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;
        AudioSource myAudioSource = null;

        private void Start()
        {
            player = GetComponent<Player>();
            myAudioSource = GetComponent<AudioSource>();
        }

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            ProcessHeal(useParams);
            PlayParticleEffect();
            myAudioSource.clip = config.GetSFX(); // TODO find way of moving audio to parent class
            myAudioSource.volume = 0.15f;
            myAudioSource.Play();
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity, transform);
            ParticleSystem myParticleSystem = prefab.GetComponent<ParticleSystem>();
            myParticleSystem.Play();
            Destroy(prefab, myParticleSystem.main.duration + 2f);
        }

        private void ProcessHeal(AbilityUseParams useParams)
        {
            //IDamageable user = gameObject.GetComponent<IDamageable>();
            //user.AdjustHealth(-config.GetHealAmount());
            player.SetHealTimer(config.GetDuration(), config.GetHealAmount());
        }
    }
}