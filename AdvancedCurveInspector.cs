#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Hypnagogia.BezierCurve
{
    [CustomEditor(typeof(AdvancedCurve))]
    public class AdvancedCurveInspector : Editor
    {
        const int stepsPerCurve = 10;
        const float directionScale = 0.5f;
        const float handleSize = 0.04f;
        const float pickSize = 0.06f;

        static Color[] modeColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        AdvancedCurve curve;
        Transform handleTransform;
        Quaternion handleRotation;
        int selectedIndex = -1;

        public override void OnInspectorGUI()
        {
            curve = target as AdvancedCurve;
            EditorGUI.BeginChangeCheck();
            bool loop = EditorGUILayout.Toggle("Loop", curve.Loop);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Toggle Loop");
                EditorUtility.SetDirty(curve);
                curve.Loop = loop;
            }

            if (selectedIndex >= 0 && selectedIndex < curve.ControlPointCount)
            {
                DrawSelectedPointInspector();
            }

            if (GUILayout.Button("Add Curve"))
            {
                Undo.RecordObject(curve, "Add Curve");
                curve.AddCurve();
                EditorUtility.SetDirty(curve);
            }
        }

        void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point");
            EditorGUI.BeginChangeCheck();
            Vector3 point = EditorGUILayout.Vector3Field("Position", curve.GetControlPoint(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.SetControlPoint(selectedIndex, point);
            }

            EditorGUI.BeginChangeCheck();
            BezierControlPointMode mode =
                (BezierControlPointMode) EditorGUILayout.EnumPopup("Mode", curve.GetControlPointMode(selectedIndex));
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Change Point Mode");
                curve.SetControlPointMode(selectedIndex, mode);
                EditorUtility.SetDirty(curve);
            }
        }

        void OnSceneGUI()
        {
            curve = target as AdvancedCurve;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local
                ? handleTransform.rotation
                : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            for (int i = 1; i < curve.ControlPointCount; i += 3)
            {
                Vector3 p1 = ShowPoint(i);
                Vector3 p2 = ShowPoint(i + 1);
                Vector3 p3 = ShowPoint(i + 2);

                Handles.color = Color.gray;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
                p0 = p3;
            }

            ShowDirections();
        }

        void ShowDirections()
        {
            Handles.color = Color.green;
            Vector3 point = curve.GetPoint(0f);
            Handles.DrawLine(point, point + curve.GetDirection(0f) * directionScale);
            int steps = stepsPerCurve * curve.CurveCount;
            for (int i = 1; i <= steps; i++)
            {
                point = curve.GetPoint(i / (float) steps);
                Handles.DrawLine(point, point + curve.GetDirection(i / (float) steps) * directionScale);
            }
        }

        Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.GetControlPoint(index));
            Handles.Label(point, index.ToString());
            float size = HandleUtility.GetHandleSize(point);
            if (index == 0)
            {
                size *= 2f;
            }

            Handles.color = modeColors[(int) curve.GetControlPointMode(index)];
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotCap))
            {
                selectedIndex = index;
                Repaint();
            }

            if (selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, handleRotation);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(curve, "Move Point");
                    EditorUtility.SetDirty(curve);
                    curve.SetControlPoint(index, handleTransform.InverseTransformPoint(point));
                }
            }

            return point;
        }
    }
}

#endif