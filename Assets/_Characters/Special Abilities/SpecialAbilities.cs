using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBarImage;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPerSec = 10f;
        [SerializeField] AudioClip outOfEnergy;

        float currentEnergyPoints;
        AudioSource audioSource;
        //Player player;

        float energyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            // player = GetComponent<Player>();
            SetCurrentMaxEnergy();
            AttachInitialAbilities();
            UpdateEnergyOrbUI();
            
        }

        private void Update()
        {
            if (currentEnergyPoints < maxEnergyPoints)
            {
                AddEnergyPoints();
                UpdateEnergyOrbUI();
            }
        }

        void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyCost = abilities[abilityIndex].GetEnergyCost();
            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public int GetNumberOfAbilities() { return abilities.Length; }

        private void AddEnergyPoints()
        {
            float pointsToAdd = regenPerSec * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
            UpdateEnergyOrbUI();
        }

        private void SetCurrentMaxEnergy() { currentEnergyPoints = maxEnergyPoints; }

        public void UpdateEnergyOrbUI()
        {
            energyBarImage.fillAmount = energyAsPercent;
        }

        
    }
}