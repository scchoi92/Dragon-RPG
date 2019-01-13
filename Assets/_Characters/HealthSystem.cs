using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBarImage;

        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] float deathVanishDelay = 5f;
        // todo maybe a parameter for character vanishing

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints;
        Animator myAnimator;
        AudioSource myAudioSource;
        CharacterMovement characterMovement;

        
        // 셀프힐용 따로 작업한 거
        float healTimer = 0f;
        [SerializeField] float healPerSec = 0f;




        void Start()
        {
            currentHealthPoints = maxHealthPoints;
            myAnimator = GetComponent<Animator>();
            myAudioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<CharacterMovement>();
        }

        public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

        void Update()
        {
            UpdateHealthBar();

            if (healthAsPercentage > Mathf.Epsilon)
            {
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

        public void AdjustHealth(float damage)
        {
            bool characterDies = (currentHealthPoints - damage <= 0);
            ReduceHealth(damage);
            if (characterDies)
            {
                StartCoroutine(KillCharacter());
            }
        }

        private void ReduceHealth(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            myAudioSource.PlayOneShot(clip);
        }

        IEnumerator KillCharacter()
        {
            StopAllCoroutines();
            characterMovement.Kill();
            myAnimator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<PlayerMovement>();
            if(playerComponent && playerComponent.isActiveAndEnabled) // relying on lazy evaluation
            {
                myAudioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
                myAudioSource.Play();
                yield return new WaitForSecondsRealtime(myAudioSource.clip.length + 1f);
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
            else // assume is enemy for now, reconsider on other npcs
            {
                Destroy(gameObject, deathVanishDelay);
            }
        }


        void UpdateHealthBar()
        {
            if(healthBarImage) // enemies might not have health bars to update
            {
                healthBarImage.fillAmount = healthAsPercentage;
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