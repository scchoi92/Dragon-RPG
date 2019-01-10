using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider re-wiring
using RPG.Core; 
using RPG.Weapons;

namespace RPG.Characters
{
    public class Enemy : MonoBehaviour, IDamageable
    {

        [SerializeField] float maxHealthPoints = 100f;
        float currentHealthPoints;

        [SerializeField] float moveRadius = 6f;
        [SerializeField] float noticeRadius = 10f;

        [SerializeField] float attackRadius = 3f;
        [SerializeField] float damagePerShot = 9f;
        [SerializeField] float firingPeriodInS = 0.5f;
        [SerializeField] float firingPeriodVariation = 0.1f;

        bool isAttacking = false;


        [SerializeField] GameObject projectileToUse;
        [SerializeField] GameObject projectileSocket;
        Vector3 aimOffset = new Vector3(0, 1.5f, 0f);

        AICharacterControl aiCharacterControl = null;
        Player player = null;
        GameObject originalPosition;
        //[SerializeField] GameObject originalPositionHolder; // give a new tag for this "holder" and find it on Start()?
        //or, make empty script, add it to the holder, so it can be found using getcomponent?


        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }

        public void TakeDamage(float damage)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            if (currentHealthPoints <= 0) { Destroy(gameObject); }
        }

        private void Start()
        {
            currentHealthPoints = maxHealthPoints;
            player = FindObjectOfType<Player>();
            aiCharacterControl = GetComponent<AICharacterControl>();
            originalPosition = new GameObject("Enemy original position");
            originalPosition.transform.position = transform.position;
            // originalPosition.transform.parent = originalPositionHolder.transform; // if I want to keep my hierarchy tidy(in game)
        }

        private void Update()
        {
            if(player.healthAsPercentage <=Mathf.Epsilon)
            { StopAllCoroutines(); Destroy(this); }
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            if (distanceToPlayer <= attackRadius && !isAttacking)
            {
                isAttacking = true;
                float randomisedDelay = Random.Range(firingPeriodInS - firingPeriodVariation, firingPeriodInS + firingPeriodVariation);
                InvokeRepeating("FireProjectile", 0.25f, randomisedDelay); //TODO switch to couroutines
                transform.LookAt(player.transform);
            }

            if (distanceToPlayer > attackRadius)
            {
                isAttacking = false;
                CancelInvoke();
            }

            //if (distanceToPlayer <= noticeRadius && distanceToPlayer >= moveRadius) // 이줄부터 ->
            //{
            //    transform.LookAt(player.transform);
            //}
            //else // <- else까지 지워도됨
            if (distanceToPlayer <= moveRadius)
            {
                aiCharacterControl.SetTarget(player.transform);
                if (distanceToPlayer <= attackRadius)
                {
                    aiCharacterControl.SetTarget(transform);
                }
            }
            else if (distanceToPlayer >= noticeRadius)
            {
                aiCharacterControl.SetTarget(originalPosition.transform);
            }



        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(255f, 0f, 0f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, attackRadius);

            Gizmos.color = new Color(0f, 0f, 255f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, moveRadius);

            Gizmos.color = new Color(50f, 50f, 50f, 0.1f);
            Gizmos.DrawWireSphere(transform.position, noticeRadius);
        }

        // TODO seperate out Character firing logic
        void FireProjectile()
        {

            GameObject newProjectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.LookRotation(player.transform.position));
            Projectile projectileComponent = newProjectile.GetComponent<Projectile>();
            projectileComponent.SetDamage(damagePerShot);
            projectileComponent.SetShooter(gameObject);

            Vector3 unitVectorToPlayer = (player.transform.position + aimOffset - projectileSocket.transform.position).normalized;
            float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
            newProjectile.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileSpeed;
        }

        private void OnDestroy()
        {
            Destroy(originalPosition);
        }
    }
}