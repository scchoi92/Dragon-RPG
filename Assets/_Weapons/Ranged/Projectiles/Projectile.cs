using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO consider re-wiring
using RPG.Core; 

namespace RPG.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float projectileSpeed;

        [SerializeField] GameObject shooter; // So can inspect when paused

        const float DESTROY_DELAY = 0.01f;

        float damageCaused;

        public void SetShooter(GameObject shooter)
        {
            this.shooter = shooter;
        }


        public void SetDamage(float damage)
        {
            damageCaused = damage;
        }


        //private void OnCollisionEnter(Collision collision)
        //{
        //    var layerCollidedWith = collision.gameObject.layer;
        //    if (layerCollidedWith != shooter.layer)
        //    {
        //        DamageIfDamageable(collision);
        //    }
        //}

        private void DamageIfDamageable(Collider collision)
        {
            Component damageableComponent = collision.gameObject.GetComponent(typeof(IDamageable));
            if (damageableComponent)
            {
                (damageableComponent as IDamageable).TakeDamage(damageCaused);
                Destroy(gameObject, DESTROY_DELAY);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (shooter == null) { return; }
            if (other.gameObject.layer != shooter.layer)
            {
                DamageIfDamageable(other);
            }
        }

        public float GetDefaultLaunchSpeed()
        {
            return projectileSpeed;
        }


        // In order to keep hierarchy clean
        private void Start()
        {
            Destroy(gameObject, 15f);
        }
    }
}