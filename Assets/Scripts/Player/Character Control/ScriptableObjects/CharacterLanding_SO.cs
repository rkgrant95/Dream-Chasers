using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace IslandFusionGames
{
    [CreateAssetMenu(fileName = "New State", menuName = "IslandFusionGames/AbilityData/Landing")]
    public class CharacterLanding_SO : StateData
    {
        public override void OnEnter(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {
            _animator.SetBool(Statics.characterJump, false);
        }

        public override void OnExit(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {

        }

        public override void OnUpdate(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {

        }

    }
}