using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IslandFusionGames
{
    [CreateAssetMenu(fileName = "New State", menuName = "IslandFusionGames/AbilityData/MoveForward")]
    public class CharacterMoveForward_SO : StateData
    {
        public float speed;

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
            if (_charControl.moveLeft && _charControl.moveRight)
            {
                _animator.SetBool(Statics.characterWalk, false);
                return;
            }
            if (!_charControl.moveLeft && !_charControl.moveRight)
            {
                _animator.SetBool(Statics.characterWalk, false);
                return;
            }

            MoveLeft(_animator, _charControl);
            MoveRight(_animator, _charControl);
        }

        private void MoveLeft(Animator _animator, CharacterControl _charControl)
        {
            if (_charControl.moveLeft)
            {
                _charControl.transform.Translate(Vector3.forward * speed * Time.deltaTime);
                _charControl.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        private void MoveRight(Animator _animator, CharacterControl _charControl)
        {
            if (_charControl.moveRight)
            {
                _charControl.transform.Translate(Vector3.forward * speed * Time.deltaTime);
                _charControl.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}