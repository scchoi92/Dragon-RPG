using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";
        public abstract void Use(GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            bool isAttatchedToUser = config.GetAttachParticleToUser();
            if (isAttatchedToUser)
            {
                Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity, transform);
            }
            else
            {
                var particleHolder = FindObjectOfType<ParticleEffectsHolder>();
                Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity, particleHolder.transform);
            }
        }

        protected void PlayAbilityAnimation()
        {
            var animatorOverrideController = GetComponent<Character>().GetOverrideController();
            var animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimation();
            animator.SetTrigger(ATTACK_TRIGGER);
        }

        protected void PlaySFX()
        {
            var myAudioSource = GetComponent<AudioSource>();
            var abilitySFX = config.GetRandomSFX(); // TODO change to random clip
            myAudioSource.PlayOneShot(abilitySFX);
        }
    }
}