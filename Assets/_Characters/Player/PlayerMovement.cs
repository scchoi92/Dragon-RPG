//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor.SceneManagement;
// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Core;


namespace RPG.Characters
{
    [SelectionBase]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] float baseDamage = 17f;
        [SerializeField] Weapon currentWeaponConfig;
        [SerializeField] AnimatorOverrideController animatorOverrideController;

        [Range(0.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 2;
        [SerializeField] GameObject critParticle;

        // Temporarily serialized for debuging

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        CharacterMovement characterMovement;
        Enemy enemy;
        Animator animator;
        SpecialAbilities abilities;

        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;
        GameObject weaponObject;

        private void Start()
        {
            characterMovement = GetComponent<CharacterMovement>();
            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseEvents();
            PutWeaponInHand(currentWeaponConfig); // todo move to weapon system
            SetAttackAnimation(); // todo move to weapon system
        }

        private void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverEnemy(Enemy enemyOver)
        {
            enemy = enemyOver;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyOver.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0, enemyOver.gameObject);
            }
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                characterMovement.SetDestination(destination);
            }
        }

        // todo to weapon
        private void SetAttackAnimation()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip(); // TODO remove const
        }

        // todo to weapon
        public void PutWeaponInHand(Weapon weaponToUse)
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

        // todo to weapon
        private GameObject RequestHeadDeco()
        {
            var headsToDeco = GetComponentsInChildren<HeadDeco>();
            int numberOfHeads = headsToDeco.Length;
            Assert.AreNotEqual(numberOfHeads, 0, "No Head found on player. Please add one.");
            Assert.IsFalse(numberOfHeads > 1, "Multiple Head scripts on player. Please remove one.");
            return headsToDeco[0].gameObject;

        }
        // todo to weapon
        private GameObject RequestDominantHand()
        {
            var dominantHands = GetComponentsInChildren<DominantHand>();
            int numberOfDominantHands = dominantHands.Length;
            Assert.AreNotEqual(numberOfDominantHands, 0, "No DominantHand found on player. Please add one.");
            Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on player. Please remove one.");
            return dominantHands[0].gameObject;
        }
        
        private void AttackTarget() // todo use co-routines
        {
            if (Time.time - lastHitTime > currentWeaponConfig.GetMinTimeBtwHits())
            {
                SetAttackAnimation();
                transform.LookAt(enemy.transform);
                animator.SetTrigger(ATTACK_TRIGGER); //TODO make const
                lastHitTime = Time.time;
            }
            
        }

        // todo to weapon
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
                Instantiate(critParticle, enemy.transform.position, Quaternion.identity);
                return GetDamageBeforeCritical() * criticalHitMultiplier;
            }
            else
            {
                return GetDamageBeforeCritical();
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= currentWeaponConfig.GetMaxAttackRange();
        }

        private void Update()
        {
            var healthPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if(healthPercentage > Mathf.Epsilon) { ScanForAbilityKeyDown(); }
            
        }

        private void ScanForAbilityKeyDown()
        {
            for(int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
        }
    }
}