using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider re-wiring
using RPG.Core;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float moveRadius = 6f;
        [SerializeField] float noticeRadius = 10f;

        bool isAttacking = false; // todo more rich state
        PlayerMovement player = null;
        float currentWeaponRange;


        GameObject originalPosition;
        GameObject originalPositionHolder;

        private void Awake()
        {
            originalPositionHolder = FindObjectOfType<OriginalPositionHolder>().gameObject;
        }

        private void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
            originalPosition = new GameObject(gameObject.name + " original position");
            originalPosition.transform.position = transform.position;
            originalPosition.transform.parent = originalPositionHolder.transform;
        }

        private void Update()
        {
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 0f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            Gizmos.color = new Color(0f, 0f, 255f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, moveRadius);

            Gizmos.color = new Color(50f, 50f, 50f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, noticeRadius);
        }

        private void OnDestroy()
        {
            Destroy(originalPosition);
        }
    }
}