using UnityEditor;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    [CustomEditor(typeof(Cap))]
    public class CapEditor : Editor
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
            _cap.TheOpenType = (OpenType)EditorGUILayout.EnumPopup(_cap.TheOpenType);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _cap.Opened = EditorGUILayout.Vector3Field("Opened", _cap.Opened);
            if (GUILayout.Button("ReSet"))
            {
                if (_cap.TheOpenType == OpenType.Rotate)
                {
                    Vector3 rot = _cap.transform.localRotation.eulerAngles;
                    float x = (rot.x > 180) ? (rot.x - 360) : rot.x;
                    float y = (rot.y > 180) ? (rot.y - 360) : rot.y;
                    float z = (rot.z > 180) ? (rot.z - 360) : rot.z;
                    _cap.Opened = new Vector3(x, y, z);
                }
                else
                {
                    _cap.Opened = _cap.transform.localPosition;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _cap.Closed = EditorGUILayout.Vector3Field("Closed", _cap.Closed);
            if (GUILayout.Button("ReSet"))
            {
                if (_cap.TheOpenType == OpenType.Rotate)
                {
                    Vector3 rot = _cap.transform.localRotation.eulerAngles;
                    float x = (rot.x > 180) ? (rot.x - 360) : rot.x;
                    float y = (rot.y > 180) ? (rot.y - 360) : rot.y;
                    float z = (rot.z > 180) ? (rot.z - 360) : rot.z;
                    _cap.Closed = new Vector3(x, y, z);
                }
                else
                {
                    _cap.Closed = _cap.transform.localPosition;
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open the cap"))
            {
                if (_cap.TheOpenType == OpenType.Rotate)
                {
                    _cap.transform.localRotation = Quaternion.Euler(_cap.Opened);
                }
                else
                {
                    _cap.transform.localPosition = _cap.Opened;
                }
            }
            if (GUILayout.Button("Close the cap"))
            {
                if (_cap.TheOpenType == OpenType.Rotate)
                {
                    _cap.transform.localRotation = Quaternion.Euler(_cap.Closed);
                }
                else
                {
                    _cap.transform.localPosition = _cap.Closed;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}
