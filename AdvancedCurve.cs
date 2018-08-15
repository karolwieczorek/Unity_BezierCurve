using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public class AdvancedCurve : MonoBehaviour
    {
        NodeProvider NodeProvider
        {
            get { return GetComponent<NodeProvider>(); }
        }
        
        [SerializeField] bool loop;

        [SerializeField] List<Node> nodes = new List<Node>();
        
        public bool Loop
        {
            get { return loop; }
            set
            {
                loop = value;
                if (value == true)
                {
                    Nodes[Nodes.Count - 1].Mode = Nodes[0].Mode;
                    SetControlPoint(0, GetPosition(0));
                }
            }
        }

        public int ControlPointCount
        {
            get { return Nodes.Count * 3 - 2; }
        }

        public int CurveCount
        {
            get { return (ControlPointCount - 1) / 3; }
        }

        public List<INode> Nodes
        {
            get { return NodeProvider.Nodes; }
        }

        public Vector3 GetControlPoint(int index)
        {
            return GetPosition(index);
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - GetPosition(index);
                if (loop)
                {
                    if (index == 0)
                    {
                        MovePosition(1, delta);
                        MovePosition(ControlPointCount - 2, delta);
                        SetPosition(ControlPointCount - 1, point);
                    }
                    else if (index == ControlPointCount - 1)
                    {
                        SetPosition(0, point);
                        MovePosition(1, delta);
                        MovePosition(index - 1, delta);
                    }
                    else
                    {
                        MovePosition(index - 1, delta);
                        MovePosition(index + 1, delta);
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        MovePosition(index - 1, delta);
                    }

                    if (index + 1 < ControlPointCount)
                    {
                        MovePosition(index + 1, delta);
                    }
                }
            }

            SetPosition(index, point);
            EnforceMode(index);
        }

        public Vector3 GetNodePosition(int index)
        {
            return GetNode(index).Point;
        }

        public Quaternion GetRotation(int index)
        {
            return Quaternion.Euler(GetNode(index).RotationEuler);
        }

        public void SetRotation(int index, Quaternion rotation)
        {
            GetNode(index).RotationEuler = rotation.eulerAngles;
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return GetNode(index).Mode;
        }

        INode GetNode(int index)
        {
            return Nodes[(index + 1) / 3];
        }

        Vector3 GetPosition(int index)
        {
            var node = GetNode(index);
            var nodeIdex = Nodes.IndexOf(node);
            var positionIndex = index - nodeIdex * 3;

            if (positionIndex == 0)
                return node.Point;
            if (positionIndex == 1)
                return node.ControlPoint0;
            if (positionIndex == -1)
                return node.ControlPoint1;

            throw new Exception("node out of range");
        }

        void SetPosition(int index, Vector3 position)
        {
            var node = GetNode(index);
            var nodeIdex = Nodes.IndexOf(node);
            var positionIndex = index - nodeIdex * 3;
            
            if (positionIndex == 0)
                node.Point = position;
            else if (positionIndex == 1)
                node.ControlPoint0 = position;
            else if (positionIndex == -1)
                node.ControlPoint1 = position;
            else
                throw new Exception("node out of range");
        }

        void MovePosition(int index, Vector3 moveAmount)
        {
            SetPosition(index, GetPosition(index) + moveAmount);
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            var node = GetNode(index);
            node.Mode = mode;
            
            if (loop && (Nodes.First() == node || Nodes.Last() == node))
            {
                Nodes.First().Mode = mode;
                Nodes.Last().Mode = mode;
            }

            EnforceMode(index);
        }

        void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = Nodes[modeIndex].Mode;
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == Nodes.Count - 1))
            {
                return;
            }

            int middleIndex = modeIndex * 3;
            int fixedIndex, enforcedIndex;
            if (index <= middleIndex)
            {
                fixedIndex = middleIndex - 1;
                if (fixedIndex < 0)
                {
                    fixedIndex = ControlPointCount - 2;
                }

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= ControlPointCount)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= ControlPointCount)
                {
                    fixedIndex = 1;
                }

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = ControlPointCount - 2;
                }
            }

            Vector3 middle = GetPosition(middleIndex);
            Vector3 enforcedTangent = middle - GetPosition(fixedIndex);
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, GetPosition(enforcedIndex));
            }

            SetPosition(enforcedIndex, middle + enforcedTangent);
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = ControlPointCount - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int) t;
                t -= i;
                i *= 3;
            }

            return transform.TransformPoint(Bezier.GetPoint(GetPosition(i), GetPosition(i + 1), GetPosition(i + 2), GetPosition(i + 3), t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = ControlPointCount - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int) t;
                t -= i;
                i *= 3;
            }

            return transform.TransformPoint(Bezier.GetFirstDerivative(GetPosition(i), GetPosition(i + 1), GetPosition(i + 2),
                       GetPosition(i + 3), t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void AddCurve()
        {
            NodeProvider.AddNode();
            Vector3 point = GetPosition(ControlPointCount - 1);
            point.x += 1f;
            SetPosition(ControlPointCount - 3, point);
            point.x += 1f;
            SetPosition(ControlPointCount - 2, point);
            point.x += 1f;
            SetPosition(ControlPointCount - 1, point);

            Nodes[Nodes.Count - 1].Mode = Nodes[Nodes.Count - 2].Mode;
            EnforceMode(ControlPointCount - 4);

            if (loop)
            {
                SetPosition(ControlPointCount - 1, GetPosition(0));
                Nodes[Nodes.Count - 1].Mode = Nodes[0].Mode;
                EnforceMode(0);
            }
        }

        public void Reset()
        {
            Nodes.Clear();
            NodeProvider.AddNode();
            NodeProvider.AddNode();
        }
    }
}