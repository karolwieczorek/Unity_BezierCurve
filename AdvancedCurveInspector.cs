#if UNITY_EDITOR

using System;
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
        static int selectedIndex = -1;
        
        static float testPosition;
        static bool showDirections;

        bool foldOutDefaultInspector;

        public override void OnInspectorGUI()
        {
            foldOutDefaultInspector = EditorGUILayout.Foldout(foldOutDefaultInspector, "Default Inspector");
            if (foldOutDefaultInspector)
            {
                DrawDefaultInspector();
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            var newValue = EditorGUILayout.Slider("Test Position", testPosition, 0f, 1f);
            if (testPosition != newValue)
            {
                testPosition = newValue;
                EditorUtility.SetDirty(target);
            }

            showDirections = EditorGUILayout.Toggle("Show Directions", showDirections);
            
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

            if (showDirections)
                ShowDirections();
            TestPositionDraw();
        }

        void TestPositionDraw()
        {
            var curve = target as AdvancedCurve;
            var position = curve.GetPoint(testPosition);
            var rotation = curve.GetRotation(testPosition);
            Handles.ArrowHandleCap(0, position, rotation, 0.25f, EventType.Repaint);
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
            if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
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
                
                DrawTool(index);
            }

            return point;
        }

        void DrawTool(int index)
        {
            switch (Tools.current)
            {
                case Tool.View:
                    break;
                case Tool.Move:
                    break;
                case Tool.Rotate:
                    DrawRotateTool(index);
                    break;
                case Tool.Scale:
                    break;
                case Tool.Rect:
                    break;
                case Tool.Transform:
                    break;
                case Tool.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void DrawRotateTool(int index)
        {
            EditorGUI.BeginChangeCheck();
            var nodeRotation = curve.GetRotation(index);
            var worldNodePosition = handleTransform.TransformPoint(curve.GetNodePosition(index));
            var newRotation = Handles.DoRotationHandle(nodeRotation, worldNodePosition);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Rotate Node");
                EditorUtility.SetDirty(curve);
                curve.SetRotation(index, newRotation);
            }
        }
    }
}

#endif