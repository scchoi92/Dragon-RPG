using System.Collections;
using UnityEngine;

namespace RPG.Characters
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;

        private void Awake()
        {
            
        }

        public abstract void Use(AbilityUseParams useParams);

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

        protected void PlaySFX()
        {
            var myAudioSource = GetComponent<AudioSource>();
            var abilitySFX = config.GetRandomSFX(); // TODO change to random clip
            myAudioSource.PlayOneShot(abilitySFX);
        }

        protected void PlayAnimation()
        {
            var myAnimator = gameObject.GetComponent<Animator>();
            myAnimator.SetTrigger(config.GetSkillAnimationTrigger());
        }

    }
}