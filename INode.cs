using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    public interface INode
    {
        Vector3 Point { get; set; }
        Vector3 ControlPoint0 { get; set; }
        Vector3 ControlPoint1 { get; set; }
        BezierControlPointMode Mode { get; set; }

        Vector3 RotationEuler { get; set; }
        float Scale { get; set; }
    }
}