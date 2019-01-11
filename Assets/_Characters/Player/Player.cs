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
using System;

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

        [Range(0.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
        [SerializeField] float criticalHitMultiplier = 2;
        [SerializeField] GameObject critParticle;

        // Temporarily serialized for dubbing
        [SerializeField] AbilityConfig[] abilities;

        const string ATTACK_TRIGGER = "Attack";
        const string DEATH_TRIGGER = "Death";

        Enemy enemy = null;
        AudioSource myaudioSource = null;
        Animator animator = null;
        CameraRaycaster cameraRaycaster = null;
        float lastHitTime = 0f;

        float healTimer = 0f;
        [SerializeField] float healPerSec = 0f;

        private void Start()
        {
            myaudioSource = GetComponent<AudioSource>();

            RegisterForMouseClick();
            SetCurrentMaxHealth();
            PutWeaponInHand();
            SetupRuntimeAnimator();
            AttachInitialAbilities();
        }

        private void AttachInitialAbilities()
        {
            for(int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachComponentTo(gameObject);
            }
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        public void AdjustHealth(float changePoints)
        {
            bool playerDies = (currentHealthPoints - changePoints <= 0);
            ReduceHealth(changePoints);
            if (playerDies)
            {
                StartCoroutine(KillPlayer());
            }
        }

        private void ReduceHealth(float damage)
        {
            myaudioSource.clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            myaudioSource.Play();
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        }

        IEnumerator KillPlayer()
        {
            myaudioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
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

        void OnMouseOverEnemy(Enemy enemyToSet)
        {
            enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyToSet.gameObject))
            {
                AttackTarget();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AttemptSpecialAbility(0);
            }
        }

        void AttemptSpecialAbility(int abilityIndex)
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

        private void AttackTarget()
        {
            if (Time.time - lastHitTime > weaponInUse.GetMinTimeBtwHits())
            {
                transform.LookAt(enemy.transform);
                animator.SetTrigger(ATTACK_TRIGGER); //TODO make const
                enemy.AdjustHealth(CalculateDamage());
                lastHitTime = Time.time;
            }
            
        }

        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) <= criticalHitChance;
            float damageBeforeCritical = baseDamage + weaponInUse.GetAdditionalWeaponDamage();
            if (isCriticalHit)
            {
                Instantiate(critParticle, enemy.transform.position, Quaternion.identity);
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponInUse.GetMaxAttackRange();
        }



        private void Update()
        {
            if (healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();

                if (healTimer > Mathf.Epsilon)
                {
                    healTimer -= Time.deltaTime;
                    StartHealing();
                }
                else
                {
                    healPerSec = 0f;
                }

            }
        }

        private void ScanForAbilityKeyDown()
        {
            for(int keyIndex =1; keyIndex < abilities.Length; keyIndex++)
            {
                if (Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }

        }

        public void SetHealTimer(float duration, float healAmount)
        {
            healTimer += duration;
            healPerSec = healAmount;
        }

        public void StartHealing()
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healPerSec * Time.deltaTime, 0f, maxHealthPoints);
        }
    }
}