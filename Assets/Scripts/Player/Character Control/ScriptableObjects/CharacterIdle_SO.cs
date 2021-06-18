using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IslandFusionGames
{
    [CreateAssetMenu(fileName = "New State", menuName = "IslandFusionGames/AbilityData/Idle")]
    public class CharacterIdle_SO : StateData
    {
        public override void OnEnter(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {

        }

        public override void OnExit(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {

        }

        public override void OnUpdate(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo)
        {
            CharacterControl charControl = _charStateBase.GetCharacterControl(_animator);
            ControlMovement(_animator, charControl);
        }

        private void ControlMovement(Animator _animator, CharacterControl _charControl)
        {
            Jump(_animator, _charControl);

            if (_charControl.moveLeft && _charControl.moveRight)
            {
                _animator.SetBool(Statics.characterWalk, false);
                return;
            }

            MoveLeft(_animator, _charControl);
            MoveRight(_animator, _charControl);
        }
        private void Jump(Animator _animator, CharacterControl _charControl)
        {
            if (_charControl.jump)
            {
                _animator.SetBool(Statics.characterJump, true);
            }
        }

        private void MoveLeft(Animator _animator, CharacterControl _charControl)
        {
            if (_charControl.moveLeft)
            {
                _animator.SetBool(Statics.characterWalk, true);
            }
        }

        private void MoveRight(Animator _animator, CharacterControl _charControl)
        {
            if (_charControl.moveRight)
            {
                _animator.SetBool(Statics.characterWalk, true);
            }
        }
    }
}