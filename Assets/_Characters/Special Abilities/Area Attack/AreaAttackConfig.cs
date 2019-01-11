using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Area Attack"))]

    public class AreaAttackConfig : AbilityConfig
    {
        [Header("Area Attack Specific")]
        [SerializeField] float effectRadius = 5f;
        [SerializeField] float damageToEachTarget = 50f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<AreaAttackBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

        public float GetDamageToEachTarget()
        {
            return damageToEachTarget;
        }

        public float GetEffectRadius()
        {
            return effectRadius;
        }

    }
}
