using System.Collections.Generic;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public abstract class NodeProvider : MonoBehaviour
    {
        public abstract List<INode> Nodes { get; }

        public abstract void AddNode();
    }
}