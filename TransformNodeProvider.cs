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

        public override void AddNode()
        {
            string name = "NewNode";
#if UNITY_EDITOR
            name = UnityEditor.GameObjectUtility.GetUniqueNameForSibling(transform, "Node (0)");
#endif
            var newNode = new GameObject(name);
            newNode.AddComponent<TransformNode>();
            newNode.transform.SetParent(transform);
        }
    }
}