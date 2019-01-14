using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider re-wiring
using RPG.Core;
using System;

namespace RPG.Characters
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] float giveUpRadius = 10f;
        [SerializeField] float waypointStopDistance = 1f;
        [SerializeField] float waypointStayTime = 0.5f;
        [SerializeField] WaypointContainer patrolPath;

        PlayerMovement player;
        Character character;
        int nextWaypointIndex = 0;
        float currentWeaponRange;
        float distanceToPlayer;


        enum State { idle, patrolling, attacking, chasing }
        State state = State.idle;

        private void Start()
        {
            player = FindObjectOfType<PlayerMovement>();
            character = GetComponent<Character>();
        }

        private void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
            if(distanceToPlayer > giveUpRadius && state != State.patrolling)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }
            if (distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
            
            if(distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines();
                state = State.attacking;
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;
            while (state == State.patrolling)
            {
                Vector3 nextWaypoinPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypoinPos);
                CycleWaypointWhenClose(nextWaypoinPos);
                yield return new WaitForSeconds(waypointStayTime); // todo parameterise
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypoinPos)
        {
            if (Vector3.Distance(transform.position, nextWaypoinPos) <= waypointStopDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            Gizmos.color = new Color(0f, 0f, 255f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);

            Gizmos.color = new Color(50f, 50f, 50f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, giveUpRadius);
        }
    }
}