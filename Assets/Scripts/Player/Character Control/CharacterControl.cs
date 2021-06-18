using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace IslandFusionGames
{
    public class CharacterControl : MonoBehaviour
    {
        [Title("Required Components")]
        [Required("Requires Animator component from character mesh")]
        public Animator animator;

        public float speed;

        public bool moveRight;
        public bool moveLeft;
        public bool jump;
    }
}