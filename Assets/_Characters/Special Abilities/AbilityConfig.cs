using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public abstract class AbilityConfig : ScriptableObject
    {

        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] bool attachParticleToUser = false;
        [SerializeField] AudioClip[] abilitySFXs = null;

        [SerializeField] string skillAnimationTrigger = null;

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo);

        public void AttachAbilityTo(GameObject gameObjectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(gameObjectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public void Use(GameObject target) { behaviour.Use(target); }

        public float GetEnergyCost() { return energyCost; }

        public GameObject GetParticlePrefab() { return particlePrefab; }

        public AudioClip GetRandomSFX() { return abilitySFXs[UnityEngine.Random.Range(0, abilitySFXs.Length)]; }

        public bool GetAttachParticleToUser() { return attachParticleToUser; }

        public string GetSkillAnimationTrigger() { return skillAnimationTrigger; }
    }
}

