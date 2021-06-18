using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IslandFusionGames
{
    [CreateAssetMenu(fileName = "New State", menuName = "IslandFusionGames/AbilityData/ForceTransition")]
    public class ForceTransition_SO : StateData
    {
        [Range(0.01f, 1f)]
        public float transitionTime;

        public override void OnEnter(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {

        }

        public override void OnExit(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {
            _animator.SetBool(Statics.animForceTransition, false);
        }

        public override void OnUpdate(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {
            if (_stateInfo.normalizedTime >= transitionTime)
            {
                _animator.SetBool(Statics.animForceTransition, true);
            }
        }
    }
}