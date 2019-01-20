using System.Collections;
using UnityEngine;
using RPG.CameraUI;

namespace RPG.Characters
{
    [SelectionBase]
    public class PlayerControl : MonoBehaviour
    {
        EnemyAI enemy;

        Character character;
        SpecialAbilities abilities;
        WeaponSystem weaponSystem;        

        private void Start()
        {
            character = GetComponent<Character>();
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
            if (Input.GetMouseButton(0) && IsTargetInRange(enemyOver.gameObject))
            {
                weaponSystem.AttackTarget(enemyOver.gameObject);
            }
            else if (Input.GetMouseButton(0) && !IsTargetInRange(enemyOver.gameObject))
            {
                StartCoroutine(ApproachAndAttack(enemyOver));
            }
            else if (Input.GetMouseButtonDown(1) && IsTargetInRange(enemyOver.gameObject))
            {
                abilities.AttemptSpecialAbility(0, enemyOver.gameObject);
            }
            else if(Input.GetMouseButtonDown(1) && !IsTargetInRange(enemyOver.gameObject))
            {
                StartCoroutine(ApproachAndPowerAttack(enemyOver));
            }
        }

        IEnumerator ApproachTarget(GameObject target)
        {
            while (!IsTargetInRange(target.gameObject))
            {
                character.SetDestination(target.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator ApproachAndAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(ApproachTarget(enemy.gameObject));
            weaponSystem.AttackTarget(enemy.gameObject);
        }
        IEnumerator ApproachAndPowerAttack(EnemyAI enemy)
        {
            yield return StartCoroutine(ApproachTarget(enemy.gameObject));
            abilities.AttemptSpecialAbility(0, enemy.gameObject);
        }

        void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if (Input.GetMouseButton(0))
            {
                weaponSystem.StopAttacking();
                character.SetDestination(destination);
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