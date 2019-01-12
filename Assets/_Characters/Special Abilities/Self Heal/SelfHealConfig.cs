using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]

    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self Heal Specific")]
        [SerializeField] float healAmount = 10f;
        [SerializeField] float duration = 1.5f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject gameObjectToAttachTo)
        {
            return gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
        }

        public float GetHealAmount()
        {
            return healAmount;
        }

        public float GetDuration()
        {
            return duration;
        }

    }
}
