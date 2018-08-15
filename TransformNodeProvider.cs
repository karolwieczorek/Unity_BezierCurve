using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public class TransformNodeProvider : NodeProvider
    {
        public override List<INode> Nodes
        {
            get
            {
                return transform.GetComponentsInChildren<INode>().ToList();
            }
        }
    }
}