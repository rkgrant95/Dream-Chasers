using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IslandFusionGames
{
    public class PlayerInput : MonoBehaviour
    {
        private CharacterControl charControl;

        private void Awake()
        {
            charControl = GetComponent<CharacterControl>();
        }

        private void Update()
        {
            if (VirtualInputManager.Instance.moveRight)
            {
                charControl.moveRight = true;
            }
            else
            {
                charControl.moveRight = false;
            }

            if (VirtualInputManager.Instance.moveLeft)
            {
                charControl.moveLeft = true;
            }
            else
            {
                charControl.moveLeft = false;
            }

            if (VirtualInputManager.Instance.jump)
            {
                charControl.jump = true;
            }
            else
            {
                charControl.jump = false;
            }
        }
    }
}