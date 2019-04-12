using System;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    [Serializable]
    public sealed class MouseLook
    {
        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool ClampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool Smooth = false;
        public float SmoothTime = 5f;
        public bool LockCursor = true;
        
        private Quaternion _characterTargetRot = Quaternion.identity;
        private Quaternion _cameraTargetRot = Quaternion.identity;

        /// <summary>
        /// …Ë÷√…„œÒª˙»∆Y÷·◊¢ ÕΩ«∂»
        /// </summary>
        public void SetLookYAngle(float angle)
        {
            _characterTargetRot = Quaternion.Euler(0f, angle, 0f);
        }

        /// <summary>
        /// …Ë÷√…„œÒª˙»∆X÷·◊¢ ÕΩ«∂»
        /// </summary>
        public void SetLookXAngle(float angle)
        {
            _cameraTargetRot = Quaternion.Euler(angle, 0f, 0f);
        }

        /// <summary>
        /// …„œÒª˙◊¢ Õ–˝◊™
        /// </summary>
        public void LookRotation(Transform character, Transform camera)
        {
            float yRot = 0;
            float xRot = 0;

            if (Main.m_Input.GetButton("MouseRight"))
            {
                yRot = Main.m_Input.GetAxis("MouseX") * XSensitivity;
                xRot = Main.m_Input.GetAxis("MouseY") * YSensitivity;
            }
            
            _characterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            _cameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

            if (ClampVerticalRotation)
                _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            if(Smooth)
            {
                character.localRotation = Quaternion.Slerp(character.localRotation, _characterTargetRot, SmoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp(camera.localRotation, _cameraTargetRot, SmoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = _characterTargetRot;
                camera.localRotation = _cameraTargetRot;
            }
        }

        /// <summary>
        /// …„œÒª˙À≤º‰◊¢ ÕµΩƒø±Í÷µ
        /// </summary>
        public void LookRotationInstant(Transform character, Transform camera)
        {
            if (ClampVerticalRotation)
                _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);

            character.localRotation = _characterTargetRot;
            camera.localRotation = _cameraTargetRot;
        }

        private Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }
    }
}
