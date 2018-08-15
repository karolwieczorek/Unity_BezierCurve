using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public class TransformNode : MonoBehaviour, INode
    {
        [SerializeField] BezierControlPointMode mode;
        [SerializeField] Vector3 controlPoint0;
        [SerializeField] Vector3 controlPoint1;
        float scale;

        public Vector3 Point
        {
            get { return transform.localPosition;}
            set { transform.localPosition = value; }
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
            get { return transform.eulerAngles;}
            set { transform.eulerAngles = value; }
        }

        public float Scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value,value,value); }
        }
    }
}