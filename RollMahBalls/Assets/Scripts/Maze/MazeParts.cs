﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MazeGen
{
    public class MazeParts : MonoBehaviour
    {
        [SerializeField]
        private MazePartDefinition[] parts;

        public MazePartDefinition[] Parts { get { return parts; } private set { } }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}