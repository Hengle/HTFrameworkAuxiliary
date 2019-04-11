using System;
using System.Collections;
using UnityEngine;

namespace HT.Framework.Auxiliary
{
    [Serializable]
    public sealed class FOVKick
    {
        public Camera Camera; 
        public float FOVIncrease = 3f; 
        public float TimeToIncrease = 1f; 
        public float TimeToDecrease = 1f; 
        public AnimationCurve IncreaseCurve;

        private float _originalFov;
        
        public void Setup(Camera camera)
        {
            CheckStatus(camera);

            Camera = camera;
            _originalFov = camera.fieldOfView;
        }
        
        private void CheckStatus(Camera camera)
        {
            if (camera == null)
            {
                throw new Exception("FOVKick camera is null, please supply the camera to the constructor");
            }

            if (IncreaseCurve == null)
            {
                throw new Exception("FOVKick Increase curve is null, please define the curve for the field of view kicks");
            }
        }
        
        public IEnumerator FOVKickUp()
        {
            float t = Mathf.Abs((Camera.fieldOfView - _originalFov)/FOVIncrease);
            while (t < TimeToIncrease)
            {
                Camera.fieldOfView = _originalFov + (IncreaseCurve.Evaluate(t/TimeToIncrease)*FOVIncrease);
                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        
        public IEnumerator FOVKickDown()
        {
            float t = Mathf.Abs((Camera.fieldOfView - _originalFov)/FOVIncrease);
            while (t > 0)
            {
                Camera.fieldOfView = _originalFov + (IncreaseCurve.Evaluate(t/TimeToDecrease)*FOVIncrease);
                t -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            Camera.fieldOfView = _originalFov;
        }
    }
}
