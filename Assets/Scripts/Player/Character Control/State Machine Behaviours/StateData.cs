using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IslandFusionGames
{
    public abstract class StateData : ScriptableObject
    {
        public float duration;

        public abstract void OnEnter(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo);
        public abstract void OnExit(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo);
        public abstract void OnUpdate(CharacterState_SMB _charStateBase, Animator _animator, AnimatorStateInfo _stateInfo);


    }
}