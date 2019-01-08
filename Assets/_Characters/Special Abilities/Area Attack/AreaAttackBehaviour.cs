using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
    public class AreaAttackBehaviour : MonoBehaviour, ISpecialAbility
    {
        AreaAttackConfig config;

        public void SetConfig(AreaAttackConfig configToSet)
        {
            this.config = configToSet;
        }

        private void Start()
        {
            Debug.Log("Area Attack Behaviour attached to " + gameObject.name);
        }

        public void Use(AbilityUseParams useParams)
        {
            print("Area Attack used by: " + gameObject.name);
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                config.GetEffectRadius(),
                Vector3.up,
                config.GetEffectRadius()
                );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                if(damageable != null)
                {
                    float damageToDeal = useParams.baseDamage + config.GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }
    }
}