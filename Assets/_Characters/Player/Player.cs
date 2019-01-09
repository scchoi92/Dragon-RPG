//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor.SceneManagement;

// TODO consider re-wiring
using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
    public class Player : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        float currentHealthPoints;

        [SerializeField] float baseDamage = 17f;
        [SerializeField] Weapon weaponInUse;
        [SerializeField] AnimatorOverrideController animatorOverrideController;


        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        // Temporarily serialized for dubbing
        [SerializeField] SpecialAbility[] abilities;

        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";

        AudioSource myaudioSource;
        Animator animator;
        CameraRaycaster cameraRaycaster;
        float lastHitTime = 0f;

        private void Start()
        {
            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            abilities[0].AttachComponentTo(gameObject);
            myaudioSource = GetComponent<AudioSource>();
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void TakeDamage(float damage)
        {
            bool playerDies = (currentHealthPoints - damage <= 0);
            ReduceHealth(damage);
            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }
        }
        private void ReduceHealth(float damage)
        {
            myaudioSource.clip = damageSounds[Random.Range(0, damageSounds.Length)];
            myaudioSource.Play();
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        }

        IEnumerator KillPlayer()
        {
            myaudioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
            myaudioSource.Play();
            animator.SetTrigger(DEATH_TRIGGER);
            yield return new WaitForSecondsRealtime(myaudioSource.clip.length + 1f);
            EditorSceneManager.LoadScene(0);
        }



        private void SetCurrentMaxHealth()
        {
            currentHealthPoints = maxHealthPoints;
        }

        private void SetupRuntimeAnimator()
        {
            animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController["DEFAULT ATTACK"] = weaponInUse.GetAttackAnimClip(); // TODO remove const
        }

        private void PutWeaponInHand()
        {
            GameObject weaponPrefab = weaponInUse.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            var weapon = Instantiate(weaponPrefab, dominantHand.transform);
            weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;

            if (weaponInUse.GetCosmeticHead() != null)
            {
                GameObject headToDeco = RequestHeadDeco();
                Instantiate(weaponInUse.GetCosmeticHead(), headToDeco.transform);
            }
            else
            {
                return;
            }

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

        private void RegisterForMouseClick()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        }

        void OnMouseOverEnemy(Enemy enemy)
        {
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                AttackTarget(enemy);
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemy.gameObject))
            {
                AttemptSpecialAbility(0, enemy);
            }
        }

        void AttemptSpecialAbility(int abilityIndex, Enemy enemy)
        {
            var energyComponent = GetComponent<Energy>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyComponent.isEnergyAvailable(energyCost)) // TODO read from scriptable object
            {
                energyComponent.ConsumeEnergy(energyCost);
                var abilityParams = new AbilityUseParams(enemy, baseDamage);
                abilities[abilityIndex].Use(abilityParams);
            }
            else
            {
                print("Not enough Energy");
            }
        }

        private void AttackTarget(Enemy enemy)
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBtwHits())
            {
                this.transform.LookAt(enemy.transform);
                animator.SetTrigger(ATTACK_TRIGGER); //TODO make const
                enemy.TakeDamage(baseDamage);
                lastHitTime = Time.time;
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }
    }
}