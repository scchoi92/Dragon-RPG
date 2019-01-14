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
        Character characterMovement;
        EnemyAI enemy;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;

        CameraRaycaster cameraRaycaster;
        

        private void Start()
        {
            characterMovement = GetComponent<Character>();
            abilities = GetComponent<SpecialAbilities>();
            weaponSystem = GetComponent<WeaponSystem>();

            RegisterForMouseEvents();
        }

        private void RegisterForMouseEvents()
        {
            CameraRaycaster cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();

            cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
            cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
        }

        void OnMouseOverEnemy(EnemyAI enemyOver)
        {
            enemy = enemyOver;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyOver.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
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

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <=  weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
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