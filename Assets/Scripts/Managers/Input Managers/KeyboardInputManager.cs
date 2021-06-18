using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace IslandFusionGames
{

    public class KeyboardInputManager : MonoBehaviour
    {

        private void Update()
        {
            MoveLeft();
            MoveRight();
            Jump();
        }

        #region Movement Directions
        public void MoveLeft()
        {
            if (Input.GetKey(KeyCode.A))
            {
                VirtualInputManager.Instance.moveLeft = true;
            }
            else
            {
                VirtualInputManager.Instance.moveLeft = false;
            }
        }

        public void MoveRight()
        {
            if (Input.GetKey(KeyCode.D))
            {
                VirtualInputManager.Instance.moveRight = true;
            }
            else
            {
                VirtualInputManager.Instance.moveRight = false;
            }
        }
        #endregion

        public void Jump()
        {
            if (Input.GetKey(KeyCode.Space))
            {
                VirtualInputManager.Instance.jump = true;
            }
            else
            {
                VirtualInputManager.Instance.jump = false;
            }
        }
    }
}