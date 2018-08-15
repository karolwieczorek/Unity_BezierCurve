using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public class SimpleNodeProvider : NodeProvider
    {
        [SerializeField] List<Node> nodes;

        public override List<INode> Nodes
        {
            get { return nodes.Cast<INode>().ToList(); }
        }

        public override void AddNode()
        {
            nodes.Add(new Node());
        }
    }
}