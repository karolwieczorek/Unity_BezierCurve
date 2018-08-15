﻿using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    [System.Serializable]
    public class Node : INode
    {
        [SerializeField] Vector3 point;
        [SerializeField] Vector3 controlPoint0;
        [SerializeField] Vector3 controlPoint1;
        [SerializeField] BezierControlPointMode mode;

        // later
        [SerializeField] Vector3 rotationEuler;
        [SerializeField] float scale;

        public Vector3 Point
        {
            get { return point; }
            set { point = value; }
        }

        public Vector3 ControlPoint0
        {
            get { return controlPoint0; }
            set { controlPoint0 = value; }
        }

        public Vector3 ControlPoint1
        {
            get { return controlPoint1; }
            set { controlPoint1 = value; }
        }

        public BezierControlPointMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

        public Vector3 RotationEuler
        {
            get { return rotationEuler; }
            set { rotationEuler = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
    }
}