﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack "))]

    public class PowerAttackConfig : SpecialAbilityConfig
    {
        [Header("Power Attack Specific")]
        [SerializeField] float rangeBonus = 1.5f;

        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            var behaviourComponent = gameObjectToAttachTo.AddComponent<PowerAttackBehaviour>();
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }
    }
}