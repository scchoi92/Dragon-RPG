using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Basic setup")]
        public Transform gripTransform;
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAnimation;

        [Header("Weapon Stats")]
        [SerializeField] float additionalWeaponDamage;
        [SerializeField] float minTimeBtwHits;
        [SerializeField] float maxAttackRange;

        [Header("Cosmetic")]
        [SerializeField] GameObject cosmeticHead;


        public GameObject GetWeaponPrefab() { return weaponPrefab; }

        public float GetAdditionalWeaponDamage() { return additionalWeaponDamage; }

        public float GetMinTimeBtwHits() { return minTimeBtwHits; } //TODO consider whether we take animation time into account

        public float GetMaxAttackRange() { return maxAttackRange; }

        public GameObject GetCosmeticHead() { return cosmeticHead; }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAnimation;
        }

        // So that asset packs cannot cause crashes
        private void RemoveAnimationEvents()
        {
            attackAnimation.events = new AnimationEvent[0];
        }
    }
}