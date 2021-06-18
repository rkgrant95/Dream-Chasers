using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace IslandFusionGames
{
    public class CharacterState_SMB : StateMachineBehaviour
    {
        public List<StateData> abilityData = new List<StateData>();

        public void UpdateAllStateData(CharacterState_SMB _charStateData, Animator _animator, AnimatorStateInfo _stateInfo)
        {
            foreach(StateData item in abilityData)
            {
                item.OnUpdate(_charStateData, _animator, _stateInfo);
            }
        }

        public override void OnStateEnter(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
        {
            foreach (StateData item in abilityData)
            {
                item.OnEnter(this, _animator, _stateInfo);
            }
        }

        public override void OnStateExit(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
        {
            foreach (StateData item in abilityData)
            {
                item.OnExit(this, _animator, _stateInfo);
            }
        }

        public override void OnStateUpdate(Animator _animator, AnimatorStateInfo _stateInfo, int _layerIndex)
        {
            UpdateAllStateData(this, _animator, _stateInfo);
        }

        private CharacterControl characterLocomotion;
        public CharacterControl GetCharacterControl(Animator _animator)
        {
            if (characterLocomotion == null)
            {
                characterLocomotion = _animator.GetComponentInParent<CharacterControl>();
            }

            return characterLocomotion;
        }
    }
}