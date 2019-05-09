using UnityEditor;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    [CustomEditor(typeof(Cap))]
    public sealed class CapEditor : Editor
    {
        private Cap _cap;

        private void OnEnable()
        {
            _cap = target as Cap;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("OpenType");
            _cap.OpenType = (Cap.CapOpenType)EditorGUILayout.EnumPopup(_cap.OpenType);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _cap.OpenValue = EditorGUILayout.Vector3Field("Open Value", _cap.OpenValue);
            if (GUILayout.Button("Get Current"))
            {
                if (_cap.OpenType == Cap.CapOpenType.Rotate)
                {
                    Vector3 rot = _cap.transform.localRotation.eulerAngles;
                    rot.x = (rot.x > 180) ? (rot.x - 360) : rot.x;
                    rot.y = (rot.y > 180) ? (rot.y - 360) : rot.y;
                    rot.z = (rot.z > 180) ? (rot.z - 360) : rot.z;
                    _cap.OpenValue = rot;
                }
                else
                {
                    _cap.OpenValue = _cap.transform.localPosition;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _cap.CloseValue = EditorGUILayout.Vector3Field("Close Value", _cap.CloseValue);
            if (GUILayout.Button("Get Current"))
            {
                if (_cap.OpenType == Cap.CapOpenType.Rotate)
                {
                    Vector3 rot = _cap.transform.localRotation.eulerAngles;
                    rot.x = (rot.x > 180) ? (rot.x - 360) : rot.x;
                    rot.y = (rot.y > 180) ? (rot.y - 360) : rot.y;
                    rot.z = (rot.z > 180) ? (rot.z - 360) : rot.z;
                    _cap.CloseValue = rot;
                }
                else
                {
                    _cap.CloseValue = _cap.transform.localPosition;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open the cap"))
            {
                if (_cap.OpenType == Cap.CapOpenType.Rotate)
                {
                    _cap.transform.localRotation = Quaternion.Euler(_cap.OpenValue);
                }
                else
                {
                    _cap.transform.localPosition = _cap.OpenValue;
                }
            }
            if (GUILayout.Button("Close the cap"))
            {
                if (_cap.OpenType == Cap.CapOpenType.Rotate)
                {
                    _cap.transform.localRotation = Quaternion.Euler(_cap.CloseValue);
                }
                else
                {
                    _cap.transform.localPosition = _cap.CloseValue;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
