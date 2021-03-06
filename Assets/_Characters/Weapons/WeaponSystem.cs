﻿using System.Collections;
using UnityEngine.Assertions;
using UnityEngine;

namespace RPG.Characters
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 17f;
        [SerializeField] WeaponConfig currentWeaponConfig;
        [Range(0f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 2;
        [SerializeField] GameObject critParticle;

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        float lastHitTime;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";
        
        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(currentWeaponConfig);
            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;

            if (target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > currentWeaponConfig.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if(characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            currentWeaponConfig = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); //empty hand
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = weaponToUse.gripTransform.localPosition;
            weaponObject.transform.localRotation = weaponToUse.gripTransform.localRotation;

            if (weaponToUse.GetCosmeticHead() != null)
            {
                GameObject headToDeco = RequestHeadDeco();
                Instantiate(weaponToUse.GetCosmeticHead(), headToDeco.transform);
            }
            else
            {
                return;
            }

        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        IEnumerator AttackTargetRepeatedly()
        {
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                var animationClip = currentWeaponConfig.GetAttackAnimClip();
                var animationClipTime = animationClip.length / character.GetAnimatorSpeedMultiplier();
                var timeToWait = animationClipTime + currentWeaponConfig.GetTimeBtwAnimationCycles();

                bool isTimeToHitAgain = Time.time - lastHitTime > timeToWait;

                if (isTimeToHitAgain)
                {
                    AttackTargetOnce();
                }
                yield return new WaitForSeconds(timeToWait);
            }
            
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = currentWeaponConfig.GetDamageDelay();
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
            lastHitTime = Time.time;
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        private void SetAttackAnimation()
        {
            if (!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide " + gameObject + "with an animator override controller");
            }
            var animatorOverrideController = character.GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip(); // TODO remove const
        }

        private GameObject RequestHeadDeco()
        {
            var headsToDeco = GetComponentsInChildren<HeadDeco>();
            int numberOfHeads = headsToDeco.Length;
            Assert.AreNotEqual(numberOfHeads, 0, "No Head found on player. Please add one.");
            Assert.IsFalse(numberOfHeads > 1, "Multiple Head scripts on player. Please remove one.");
            return headsToDeco[0].gameObject;

        }

        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No DominantHand found on player. Please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on player. Please remove one.");
            return dominantHands[0].gameObject;
        }

        public WeaponConfig GetCurrentWeapon()
        {
            return currentWeaponConfig;
        }

        public float GetDamageBeforeCritical()
        {
            return baseDamage + currentWeaponConfig.GetAdditionalWeaponDamage();
        }

        // todo to weapon
        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            if (isCriticalHit)
            {
                Instantiate(critParticle, target.transform.position, Quaternion.identity);
                return GetDamageBeforeCritical() * criticalHitMultiplier;
            }
            else
            {
                return GetDamageBeforeCritical();
            }
        }
    }
}