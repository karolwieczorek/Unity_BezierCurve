using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    [System.Serializable]
    public class Node
    {
        public Vector3 point;
        public Vector3 controlPoint0;
        public Vector3 controlPoint1;
        public BezierControlPointMode mode;

        // later
        public Quaternion rotation;
        public float scale;
    }
}