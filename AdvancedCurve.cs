using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public class AdvancedCurve : MonoBehaviour
    {
        [SerializeField] Vector3[] points;

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
                    nodes[nodes.Count - 1].mode = nodes[0].mode;
                    SetControlPoint(0, points[0]);
                }
            }
        }

        public int ControlPointCount
        {
            get { return points.Length; }
        }

        public Vector3 GetControlPoint(int index)
        {
            return points[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            if (index % 3 == 0)
            {
                Vector3 delta = point - points[index];
                if (loop)
                {
                    if (index == 0)
                    {
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = point;
                    }
                    else if (index == points.Length - 1)
                    {
                        points[0] = point;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                    {
                        points[index - 1] += delta;
                    }

                    if (index + 1 < points.Length)
                    {
                        points[index + 1] += delta;
                    }
                }
            }

            points[index] = point;
            EnforceMode(index);
        }

        public BezierControlPointMode GetControlPointMode(int index)
        {
            return GetNode(index).mode;
        }

        Node GetNode(int index)
        {
            return nodes[(index + 1) / 3];
        }

        Vector3 GetPosition(int index)
        {
            var node = GetNode(index);
            var nodeIdex = nodes.IndexOf(node);
            var positionIndex = index - nodeIdex * 3;

            if (positionIndex == 0)
                return node.point;
            if (positionIndex == 1)
                return node.controlPoint0;
            if (positionIndex == -1)
                return node.controlPoint1;

            throw new Exception("node out of range");
        }

        void SetPosition(int index, Vector3 position)
        {
            var node = GetNode(index);
            var nodeIdex = nodes.IndexOf(node);
            var positionIndex = index - nodeIdex * 3;
            
            if (positionIndex == 0)
                node.point = position;
            if (positionIndex == 1)
                node.controlPoint0 = position;
            if (positionIndex == -1)
                node.controlPoint1 = position;
            
            throw new Exception("node out of range");
        }

        public void SetControlPointMode(int index, BezierControlPointMode mode)
        {
            int modeIndex = (index + 1) / 3;
            nodes[modeIndex].mode = mode;
            if (loop)
            {
                if (modeIndex == 0)
                {
                    nodes[nodes.Count- 1].mode = mode;
                }
                else if (modeIndex == nodes.Count - 1)
                {
                    nodes[0].mode = mode;
                }
            }

            EnforceMode(index);
        }

        void EnforceMode(int index)
        {
            int modeIndex = (index + 1) / 3;
            BezierControlPointMode mode = nodes[modeIndex].mode;
            if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == nodes.Count - 1))
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
                    fixedIndex = points.Length - 2;
                }

                enforcedIndex = middleIndex + 1;
                if (enforcedIndex >= points.Length)
                {
                    enforcedIndex = 1;
                }
            }
            else
            {
                fixedIndex = middleIndex + 1;
                if (fixedIndex >= points.Length)
                {
                    fixedIndex = 1;
                }

                enforcedIndex = middleIndex - 1;
                if (enforcedIndex < 0)
                {
                    enforcedIndex = points.Length - 2;
                }
            }

            Vector3 middle = points[middleIndex];
            Vector3 enforcedTangent = middle - points[fixedIndex];
            if (mode == BezierControlPointMode.Aligned)
            {
                enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
            }

            points[enforcedIndex] = middle + enforcedTangent;
        }

        public int CurveCount
        {
            get { return (points.Length - 1) / 3; }
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int) t;
                t -= i;
                i *= 3;
            }

            return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Length - 4;
            }
            else
            {
                t = Mathf.Clamp01(t) * CurveCount;
                i = (int) t;
                t -= i;
                i *= 3;
            }

            return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2],
                       points[i + 3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }

        public void AddCurve()
        {
            nodes.Add(new Node());
            Vector3 point = points[points.Length - 1];
            Array.Resize(ref points, points.Length + 3);
            point.x += 1f;
            points[points.Length - 3] = point;
            point.x += 1f;
            points[points.Length - 2] = point;
            point.x += 1f;
            points[points.Length - 1] = point;

            nodes[nodes.Count - 1].mode = nodes[nodes.Count - 2].mode;
            EnforceMode(points.Length - 4);

            if (loop)
            {
                points[points.Length - 1] = points[0];
                nodes[nodes.Count - 1].mode = nodes[0].mode;
                EnforceMode(0);
            }
        }

        public void Reset()
        {
            points = new Vector3[]
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f)
            };

            nodes = new List<Node>()
            {
                new Node(),
                new Node()
            };
        }
    }
}